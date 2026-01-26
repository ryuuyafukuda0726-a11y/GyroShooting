using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

//ハムスター用スクリプトクラス
public class Hamster : MonoBehaviour
{
    //アニメーション用変数
    private Animator myAnimator;
    //ターゲット判定用変数
    private Ray ray;
    private RaycastHit hit;
    [SerializeField]
    private float trapRange = 0.0f;
    //ナビ用変数
    private NavMeshAgent agent;
    private Transform target;
    private Transform player;
    private List<GameObject> trapList;
    private Transform trap;
    //HP用変数
    [SerializeField]
    private int maxHp = 0;
    private int hp = 0;
    //移動用変数
    private float idleInterval = 0.0f;
    [SerializeField]
    private float moveSpeed = 0.0f;
    private bool isWalk = false;
    [SerializeField]
    private float maxInterval = 0.0f;
    [SerializeField]
    private float minInterval = 0.0f;
    //攻撃用変数
    [SerializeField]
    private float attackDistance = 0.0f;
    [SerializeField]
    private int power = 0;
    private bool isAttack = false;
    private Transform attackTarget;
    //空腹度UI用変数
    [SerializeField]
    private GameObject hungryGagePrefab;
    private GameObject hungryGage;
    private HungryGage hungryGageScript;
    //アイテム用変数
    [SerializeField]
    private float itemDropProbability = 0.0f;
    //コールバック用メソッド
    public Action<Transform> destroyCallBack;
    public Action<Vector3> itemDropCallBack;
    //時間管理用変数
    private float myTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        //agent.speed = moveSpeed;
        //myAnimator = transform.GetChild(0).GetComponent<Animator>();
        //trapList = null;
    }

    //初期設定用メソッド
    public void Init(Transform inCanvas)
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        myAnimator = transform.GetChild(0).GetComponent<Animator>();
        trapList = null;

        hp = maxHp;
        //agent.isStopped = true;
        hungryGage = GameObject.Instantiate(hungryGagePrefab, inCanvas);
        hungryGageScript = hungryGage.GetComponent<HungryGage>();
        hungryGageScript.Init();
        hungryGage.SetActive(false);
    }

    //コールバックの設定用メソッド
    public void SetCallBack()
    {
        transform.GetChild(0).GetComponent<Hamster_Model>().AttackCallBack = AttackCallBack;
    }

    //ターゲットの設定用メソッド
    public void SetTarget(Transform inTarget)
    {
        target = inTarget;
    }

    //プレイヤーの設定用メソッド
    public void SetPlayer(Transform inPlayer)
    {
        player = inPlayer;
    }

    //プレイヤーの発見処理用メソッド
    private bool PlayerDiscovery()
    {
        Vector3 rayVec = player.position - transform.position;
        ray = new Ray(transform.position, rayVec);
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == player.gameObject) return true;
        }
        return false;
    }

    //トラップの発見処理用メソッド
    private bool FeedingBoxDiscovery()
    {
        trap = null;
        if (trapList == null || trapList.Count <= 0) return false;
        trap = trapList[0].transform;
        float distance = Vector3.Distance(trap.position, transform.position);
        for (int i = 1; i < trapList.Count; i++)
        {
            float nextDistance = Vector3.Distance(trapList[i].transform.position, transform.position);
            if (distance > nextDistance) trap = trapList[i].transform;
        }
        if (Vector3.Distance(trap.position, transform.position) > trapRange)
        {
            trap = null;
            return false;
        }
        agent.destination = trap.position;
        return true;
    }

    //目的地設定用メソッド
    private void SetDestination()
    {
        if (!FeedingBoxDiscovery())
        {
            agent.destination = PlayerDiscovery() ? player.position : target.position;
        }
    }

    //アイドル用メソッド
    private void Idle()
    {
        if (isWalk) return;
        if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
        // アニメーションが再生中の場合
        if (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) return;
        agent.isStopped = false;
        idleInterval = UnityEngine.Random.Range(minInterval, maxInterval);
        isWalk = true;
    }

    //移動のインターバル用メソッド
    private void MoveInterval()
    {
        myTime += Time.deltaTime;
        if(myTime > idleInterval)
        {
            myTime = 0.0f;
            agent.isStopped = true;
            isWalk = false;
        }
    } 

    //移動用メソッド
    private void Move()
    {
        myAnimator.SetBool("Walk", isWalk);
        if (!isWalk) return;
        MoveInterval();
    }

    //攻撃距離の判定用メソッド
    private Transform CheckAttackDistance()
    {
        if (trap != null)
        {
            float trapDistance = Vector3.Distance(transform.position, trap.position);
            //Debug.Log("距離 : " + trapDistance);
            if (trapDistance < attackDistance) return trap;
            else return null;
        }
        float playerDistance = Vector3.Distance(transform.position, player.position);
        float targetDistance = Vector3.Distance(transform.position, target.position);
        if (playerDistance < targetDistance)
        {
            if (playerDistance < attackDistance) return player;
        }
        else if (targetDistance < attackDistance) return target;
        return null;
    }

    //攻撃開始用メソッド
    private void AttackStart()
    {
        attackTarget = CheckAttackDistance();
        if (attackTarget == null) return;
        agent.isStopped = true;
        transform.rotation = 
            Quaternion.LookRotation(attackTarget.position - transform.position);
        isAttack = true;
    }

    //攻撃時コールバック用メソッド
    private void AttackCallBack()
    {
        if (attackTarget == player) player.GetComponent<Player>().Damage(power);
        else if (attackTarget == target) target.GetComponent<SunFlower>().Damage(power);
        else if (attackTarget == trap) {
            FeedingBox box = trap.GetComponent<FeedingBox>();
            Damage(box.GetDamageValue());
            box.durabilityDown();
        }
    }

    //攻撃用メソッド
    private void Attack()
    {
        isAttack = false;
        AttackStart();
        myAnimator.SetBool("Attack", isAttack);
    }

    //空腹ゲージ用メソッド
    private void HungryGage()
    {
        hungryGageScript.SetPos(transform);
        hungryGageScript.ControlDisplayTime();
    }

    //プレイ用メソッド
    public void Play()
    {
        SetDestination();
        Attack();
        Move();
        Idle();
        HungryGage();
    }

    //トラップのリスト登録用メソッド
    public void SetTrapList(List<GameObject> inTrapList)
    {
        trapList = inTrapList;
    }

    //撃破時用メソッド
    private void Destroy()
    {
        destroyCallBack(transform);
        transform.position = transform.parent.position;
        hungryGage.SetActive(false);
        gameObject.SetActive(false);
    }

    //ドロップ確率確認用メソッド
    private bool CheckDropProbability()
    {
        float min = 0.0f;
        float max = 1.0f;
        float probability = UnityEngine.Random.Range(min, max);
        return probability < itemDropProbability ? true : false;
    }

    //アイテムドロップ用メソッド
    private void ItemDrop()
    {
        if (!CheckDropProbability()) return;
        itemDropCallBack(transform.position);
    }

    //体力減少用メソッド
    private void Damage(int inDamageValue)
    {
        hp -= inDamageValue;
        hungryGageScript.Display(hp * 10);
        if (hp > 0) return;
        ItemDrop();
        Destroy();
    }

    //当たり判定用メソッド
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            SunflowerSeed seed = other.transform.GetComponent<SunflowerSeed>();
            seed.DisappearanceAndHitDetection();
            Damage(seed.GetDamageValue());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
