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
    //ナビ用変数
    private NavMeshAgent agent;
    private Transform target;
    private Transform player;
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
    //コールバック用メソッド
    public Action<Transform> destroyCallBack;
    //時間管理用変数
    private float myTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myAnimator = transform.GetChild(0).GetComponent<Animator>();
    }

    //初期設定用メソッド
    public void Init(Transform inCanvas)
    {
        hp = maxHp;
        hungryGage = GameObject.Instantiate(hungryGagePrefab, inCanvas);
        hungryGageScript = hungryGage.GetComponent<HungryGage>();
        hungryGageScript.Init();
        hungryGage.SetActive(false);
    }

    //コールバックの設定用メソッド
    public void SetCallBack()
    {
        transform.GetChild(0).GetComponent<Hamster_Model>().AttackCallBack = Damage;
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

    ////トラップの発見処理用メソッド
    //private bool FeedingBoxDiscovery()
    //{
    //    Vector3 rayVec = 
    //}

    //目的地設定用メソッド
    private void SetDestination()
    {
        agent.destination = PlayerDiscovery() ? player.position : target.position;
    }

    //アイドル用メソッド
    private void Idle()
    {
        if (isWalk) return;
        if (!myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
        // アニメーションが再生中の場合
        if (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1) return;
        agent.speed = moveSpeed;
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
            agent.speed = 0.0f;
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

    //距離の判定用メソッド
    private Transform CheckDistance()
    {
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
        attackTarget = CheckDistance();
        if (attackTarget == null) return;
        agent.speed = 0.0f;
        transform.rotation = Quaternion.LookRotation(attackTarget.position - transform.position);
        isAttack = true;
    }

    //ダメージ用メソッド
    private void Damage()
    {
        if (attackTarget == player) player.GetComponent<Player>().Damage(power);
        else if (attackTarget == target) target.GetComponent<SunFlower>().Damage(power);
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

    //当たり判定用メソッド
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Bullet")
        {
            other.transform.GetComponent<SunflowerSeed>().DisappearanceAndHitDetection();
            hp--;
            hungryGageScript.Display(hp * 10);
            if (hp > 0) return;
            destroyCallBack(transform);
            transform.position = transform.parent.position;
            hungryGage.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
