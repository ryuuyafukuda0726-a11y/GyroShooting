using UnityEngine;
using UnityEngine.AI;

//ハムスター用スクリプトクラス
public class Hamster : MonoBehaviour
{
    //ターゲット判定用変数
    private Ray ray;
    private RaycastHit hit;
    //ナビ用変数
    private NavMeshAgent agent;
    private Transform target;
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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

    //プレイ用メソッド
    public void Play()
    {
        SetDestination();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
