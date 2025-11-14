using UnityEditor.Rendering;
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
    private bool isAttack = false;
    private Transform attackTarget;
    //時間管理用変数
    private float myTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myAnimator = transform.GetChild(0).GetComponent<Animator>();
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
        idleInterval = Random.Range(minInterval, maxInterval);
        isWalk = true;
    }

    //移動のインターバル用変数
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
        if (myAnimator.GetBool("Attack")) return;
        Damage();
    }

    //ダメージ用メソッド
    public void Damage()
    {
        //Debug.Log("Damage");
        if (attackTarget == player) player.GetComponent<Player>().Damage();
        else if (attackTarget == target) target.GetComponent<SunFlower>().Damage();
    }

    //攻撃用メソッド
    private void Attack()
    {
        isAttack = false;
        AttackStart();
        myAnimator.SetBool("Attack", isAttack);
    }

    //プレイ用メソッド
    public void Play()
    {
        SetDestination();
        Attack();
        Move();
        Idle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
