using MagicOnionStudy.Shared;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    //通信用メソッド
    private MagicOnionController myControllerInstance;

    //コールバックの設定用メソッド
    public void SetCallBack(Action<Vector3> inAction)
    {
        int size = hamsterObjects.Length;
        for(int i = 0; i < size; i++)
        {
            hamsterObjects[i].GetComponent<Hamster>().itemDropCallBack
                = inAction;
        }
    }

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
        myControllerInstance = MagicOnionController.GetInstance;
        myControllerInstance.receiver.OnHamsterMoveCallBack = ReceptionHamsterMoveInput;
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
        int index = UnityEngine.Random.Range(0, spawnPoints.childCount);
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

    //ハムスターの位置情報を送信するメソッド
    private void HamsterDataTransmission()
    {
        if (!CheckHost()) return;
        int size = hamsters.Count;
        if (size <= 0) return;
        MyVector3[] pos = new MyVector3[size];
        MyQuaternion[] rot = new MyQuaternion[size];
        for (int i = 0; i < size; i++)
        {
            pos[i] = hamsterObjects[i].transform.position.ToMyVector3();
            rot[i] = hamsterObjects[i].transform.rotation.ToMyQuaternion();
        }
        myControllerInstance.me.HamsterDataTransmission(size, pos, rot);
    }

    //ハムスターのプレイ中処理用メソッド
    private void HamsterPlay()
    {
        int size = hamsters.Count;
        if (size <= 0) return;
        for (int i = 0; i < size; i++)
        {
            hamsters[i].GetComponent<Hamster>().Play();
        }
        HamsterDataTransmission();
    }

    //受信したハムスターの移動を入力用メソッド
    private void ReceptionHamsterMoveInput(int inCount, 
                                           MyVector3[] position, 
                                           MyQuaternion[] quaternion)
    {
        for (int i = 0; i < inCount; i++) 
        {
            hamsterObjects[i].SetActive(true);
            hamsterObjects[i].transform.position = position[i].ToUnityVector3();
            hamsterObjects[i].transform.rotation = quaternion[i].ToUnityQuaternion();
        }
    }

    //ホストの確認用メソッド
    private bool CheckHost()
    {
        if (myControllerInstance != null)
        {
            if (myControllerInstance.isHost) return true;
        }
        return false;
    }

    //プレイ用メソッド
    public void Play()
    {
        if (myControllerInstance != null &&
           (myControllerInstance.isMulti && 
           !myControllerInstance.isHost)) return;
        HamsterPlay();
        if (hamsters.Count >= spawnHamsterCount) return;
        if (SpawnInterval()) SpawnHamster();
    }

    //撃破時用メソッド
    private void DestroyHamster(Transform inTransform)
    {
        for(int i = 0; i < hamsters.Count; i++)
        {
            if (hamsters[i].gameObject != inTransform.gameObject) continue;
            hamsters.RemoveAt(i);
        }
    }

    //トラップのリスト登録用変数
    private void SetTrapList()
    {
        for (int i = 0; i < hamsters.Count; i++)
        {
            Hamster hamster = hamsters[i].GetComponent<Hamster>();
            if (trapList == null) hamster.SetTrapList(null);
            else hamster.SetTrapList(trapList);
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
