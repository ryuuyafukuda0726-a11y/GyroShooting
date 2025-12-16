using System.Collections.Generic;
using UnityEngine;

//ハムスターを管理するマネージャースクリプトクラス
public class HamusuterManager
{
    //キャンバス用変数
    private Transform canvas;
    //ターゲット用変数
    private Transform target;
    //プレイヤー用変数
    private Transform player;
    //スポーン位置用変数
    private Transform spawnPoints;
    //スポーンインターバル用変数
    private const float spawnInterval = 5.0f;
    private float myTime = 0.0f;
    //ハムスター用変数
    private int spawnHamsterCount;
    private int hamsterMaxCount = 0;
    private int hamsterNumber = 0;
    private GameObject hamsterPrefab;
    private GameObject[] hamsterObjects;
    private GameObject hamsterParent;
    private List<GameObject> hamsters = new List<GameObject>();
    //private List<Hamster> hamsterScripts = new List<Hamster>();
    //トラップ用変数
    private List<GameObject> trapList;

    //ハムスターの生成用メソッド
    private void CreateHamster()
    {
        hamsterParent = new GameObject("HamsterParent");
        hamsterObjects = new GameObject[hamsterMaxCount];
        for(int i = 0; i < hamsterMaxCount; i++)
        {
            hamsterObjects[i] = GameObject.Instantiate(hamsterPrefab, hamsterParent.transform);
            hamsterObjects[i].GetComponent<Hamster>().Init(canvas);
            hamsterObjects[i].SetActive(false);
        }
    }

    //初期設定用メソッド
    public void Init()
    {
        CreateHamster();
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
        int index = Random.Range(0, spawnPoints.childCount);
        Vector3 pos = spawnPoints.GetChild(index).position;
        return pos;
    }

    //ハムスターが出現済みかの確認用メソッド
    private bool CheckActiveHamster()
    {
        if (hamsters.Count == 0) return true;
        for (int i = 0; i < hamsters.Count; i++)
        {
            if (hamsters[i] == hamsterObjects[hamsterNumber]) return false;
        }
        return true;
    }

    //出現したハムスターのリスト登録用メソッド
    private void SetListSpawnHamster()
    {
        hamsterObjects[hamsterNumber].SetActive(true);
        Hamster hamsterScript = hamsterObjects[hamsterNumber].GetComponent<Hamster>();
        hamsterObjects[hamsterNumber].transform.position = SetPos();
        hamsterScript.SetTarget(target);
        hamsterScript.SetPlayer(player);
        hamsterScript.SetCallBack();
        hamsterScript.destroyCallBack = DestroyHamster;
        hamsters.Add(hamsterObjects[hamsterNumber]);
        SetTrapList();
        //hamsterScripts.Add(hamsterScript);
    }

    //ハムスターの出現用メソッド
    private void SpawnHamster()
    {
        bool isFlag = false;
        while (!isFlag)
        {
            isFlag = CheckActiveHamster();
            if(isFlag) SetListSpawnHamster();
            hamsterNumber++;
            if (hamsterNumber == hamsterMaxCount) hamsterNumber = 0;
        }        
    }

    //ハムスターのプレイ中処理用メソッド
    private void HamsterPlay()
    {
        int size = hamsters.Count;
        for(int i = 0; i < size; i++)
        {
            hamsters[i].GetComponent<Hamster>().Play();
        }
    }

    //プレイ用メソッド
    public void Play()
    {
        HamsterPlay();
        if (hamsters.Count < spawnHamsterCount)
        {
            if (SpawnInterval()) SpawnHamster();
        }
    }

    //撃破時用メソッド
    private void DestroyHamster(Transform inTransform)
    {
        for(int i = 0; i < hamsters.Count; i++)
        {
            if (hamsters[i].gameObject != inTransform.gameObject) continue;
            hamsters.RemoveAt(i);
            //hamsterScripts.RemoveAt(i);
        }
    }

    //トラップのリスト登録用変数
    private void SetTrapList()
    {
        if (trapList == null) return;
        for (int i = 0; i < hamsters.Count; i++)
        {
            hamsters[i].GetComponent<Hamster>().SetTrapList(trapList);
        }
    }

    //トラップリストの登録コールバック用メソッド
    public void SetTrapListCallBack(List<GameObject> inList)
    {
        trapList = inList;
        if (hamsters == null) return;
        SetTrapList();
    }

    //コンストラクター
    public HamusuterManager(Transform inTargets, 
                            int inSpawnCount, 
                            GameObject inHamsterPrefab, 
                            Transform inPlayer, 
                            Transform inSpawnPoint,
                            Transform inCanvas)
    {
        target = inTargets;
        spawnHamsterCount = inSpawnCount;
        hamsterMaxCount = spawnHamsterCount * 2;
        hamsterPrefab = inHamsterPrefab;
        player = inPlayer;
        spawnPoints = inSpawnPoint;
        canvas = inCanvas;
    }
}
