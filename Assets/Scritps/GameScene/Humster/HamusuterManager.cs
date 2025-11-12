using System.Collections.Generic;
using UnityEngine;

//ハムスターを管理するマネージャースクリプトクラス
public class HamusuterManager
{
    //ターゲット用変数
    private Transform target;
    //プレイヤー用変数
    private Transform player;
    //スポーン位置用変数
    private Transform spawnPoint;
    //スポーンインターバル用変数
    private const float spawnInterval = 5.0f;
    private float myTime = 0.0f;
    //ハムスター用変数
    private int spawnMonsterCount;
    private GameObject hamsterPrefab;
    private List<GameObject> hamsters = new List<GameObject>();
    private List<Hamster> hamsterScripts = new List<Hamster>();
    private GameObject hamsterParent;

    //初期設定用メソッド
    public void Init()
    {
        hamsterParent = new GameObject("HamsterParent");
    }

    //生成インターバル用メソッド
    private bool SpawnInterval()
    {
        myTime += Time.deltaTime;
        if(myTime > spawnInterval)
        {
            myTime = 0.0f;
            return true;
        }
        return false;
    }

    //配置用メソッド
    private Vector3 SetPos()
    {
        int index = Random.Range(0, spawnPoint.childCount);
        Vector3 pos = spawnPoint.GetChild(index).position;
        return pos;
    }

    //ハムスターの生成用メソッド
    private void SpawnHamster()
    {
        GameObject hamster = GameObject.Instantiate(hamsterPrefab, hamsterParent.transform);
        Hamster hamsterScript = hamster.GetComponent<Hamster>();
        hamster.transform.position = SetPos();
        hamsterScript.SetTarget(target);
        hamsterScript.SetPlayer(player);
        hamsters.Add(hamster);
        hamsterScripts.Add(hamsterScript);
    }

    //ハムスターのプレイ中処理用メソッド
    private void HamsterPlay()
    {
        int size = hamsters.Count;
        for(int i = 0; i < size; i++)
        {
            hamsterScripts[i].Play();
        }
    }

    //プレイ用メソッド
    public void Play()
    {
        HamsterPlay();
        if (hamsters.Count >= spawnMonsterCount) return;
        if (SpawnInterval()) SpawnHamster();
    }

    //コンストラクター
    public HamusuterManager(Transform inTargets, 
                            int inSpawnCount, 
                            GameObject inHamsterPrefab, 
                            Transform inPlayer, 
                            Transform inSpawnPoint)
    {
        target = inTargets;
        spawnMonsterCount = inSpawnCount;
        hamsterPrefab = inHamsterPrefab;
        player = inPlayer;
        spawnPoint = inSpawnPoint;
    }
}
