using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;

//ハムスター用スクリプトクラス
public class Hamster : MonoBehaviour
{
    [NonSerialized]
    public int number = 0;
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
    private List<GameObject> trapList = new List<GameObject>();
    private Transform trap;
    //HP用変数
    [SerializeField]
    private int maxHp = 0;
    [NonSerialized]
    public int hp = 0;
    private int beforHp = 0;
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
    //通信用変数
    private MagicOnionController myControllerInstance;
    //時間管理用変数
    private float myTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myControllerInstance = MagicOnionController.GetInstance;
        beforHp = maxHp;
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
        Debug.Log("トラップ発見");
        return true;
    }

    Vector3 A()
    {
        //Debug.Log("プレイヤー発見");
        return player.position;
    }

    Vector3 B()
    {
        //Debug.Log("ひまわり発見");
        return target.position;
    }

    //目的地設定用メソッド
    private void SetDestination()
    {
        if (!FeedingBoxDiscovery())
        {
            agent.destination = PlayerDiscovery() ? A() : B();
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

    //ノックバックのベクトル生成用メソッド
    private Vector3 CreateKnockBack()
    {
        return (player.position - transform.position).normalized;
    }

    //攻撃時コールバック用メソッド
    private void AttackCallBack()
    {
        if (attackTarget == player) player.GetComponent<Player>().Damage(power, CreateKnockBack());
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

    //前フレームとの体力の差を確認
    private void CheckBeforFrameHP()
    {
        Debug.Log("体力 : " + hp);
        if (!myControllerInstance.isMulti || myControllerInstance.isHost) return;
        if (hp == beforHp) return;
        Debug.Log("体力減少");
        beforHp = hp;
        hungryGageScript.Display(hp * 10);        
    }

    //プレイ用メソッド
    public void Play()
    {
        HungryGage();
        CheckBeforFrameHP();
        if (myControllerInstance.isMulti && !myControllerInstance.isHost) return;
        SetDestination();
        Attack();
        Move();
        Idle();
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
        itemDropCallBack(transform.position + Vector3.up * 0.6f);
    }

    //体力減少用メソッド
    private void Damage(int inDamageValue)
    {
        hp -= inDamageValue;
        hungryGageScript.Display(hp * 10);
        Debug.Log("ダメージ : " + inDamageValue);
        Debug.Log("体力 : " + hp);
        if (hp > 0) return;
        ItemDrop();
        Destroy();
    }

    //接触した種の消滅情報を送信する
    private void HitSeedDestroyTransmission(SunflowerSeed seed)
    {
        if (!myControllerInstance.isMulti) return;
        myControllerInstance.me.SeedDestroyTransmission(seed.number);
    }

    //当たり判定用メソッド
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            SunflowerSeed seed = other.transform.GetComponent<SunflowerSeed>();
            HitSeedDestroyTransmission(seed);
            if (myControllerInstance.isMulti && !myControllerInstance.isHost) return;
            seed.DisappearanceAndHitDetection();
            Damage(seed.GetDamageValue());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
