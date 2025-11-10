using UnityEngine;
using UnityEngine.AI;

//ハムスター用スクリプトクラス
public class Humster : MonoBehaviour
{
    //ナビ用変数
    private NavMeshAgent agent;
    private Transform target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    //ターゲットの設定用メソッド
    public void SetTarget()
    {

    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.position;
    }
}
