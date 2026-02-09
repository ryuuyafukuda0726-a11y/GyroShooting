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
        CreateHamster();
        if (!myControllerInstance.isMulti || myControllerInstance.isHost) return;
        myControllerInstance.receiver.OnHamsterMoveCallBack
            = ReceptionHamsterMoveInput;
        myControllerInstance.receiver.OnHamsterDestroyCallBack
            = HamsterDestroy;
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
        hamsterScript.number = hamsterNumber;
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

    //情報を送信するハムスターの選別用メソッド
    private List<int> CheckTransmissionHamsterCount()
    {
        int size = hamsterObjects.Length;
        List<int> numbers = new List<int>();
        for (int i = 0; i < size; i++)
        {
            if (!hamsterObjects[i].activeSelf) continue;
            numbers.Add(i);
        }
        return numbers;
    }

    //ハムスターの位置情報を送信するメソッド
    private void HamsterDataTransmission()
    {
        if (!myControllerInstance.isHost) return;
        List<int> numbers = CheckTransmissionHamsterCount();
        if (numbers.Count <= 0) return;
        MyVector3[] pos = new MyVector3[numbers.Count];
        MyQuaternion[] rot = new MyQuaternion[numbers.Count];
        int[] hp = new int[numbers.Count];
        for (int i = 0; i < numbers.Count; i++)
        {
            pos[i] = hamsterObjects[numbers[i]].transform.position.ToMyVector3();
            rot[i] = hamsterObjects[numbers[i]].transform.rotation.ToMyQuaternion();
            hp[i] = hamsterObjects[numbers[i]].GetComponent<Hamster>().hp;
        }
        myControllerInstance.me.HamsterDataTransmission(numbers, pos, rot, hp);
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
    private void ReceptionHamsterMoveInput(List<int> inNumber, 
                                           MyVector3[] position, 
                                           MyQuaternion[] quaternion,
                                           int[] inHp)
    {
        for (int i = 0; i < inNumber.Count; i++) 
        {
            hamsterObjects[inNumber[i]].SetActive(true);
            hamsterObjects[inNumber[i]].transform.position = position[i].ToUnityVector3();
            hamsterObjects[inNumber[i]].transform.rotation = quaternion[i].ToUnityQuaternion();
            hamsterObjects[inNumber[i]].GetComponent<Hamster>().hp = inHp[i];
            hamsterObjects[inNumber[i]].GetComponent<Hamster>().Play();
        }
    }

    //ハムスター撃破時コールバック
    private void HamsterDestroy(int number)
    {
        GameObject hamster = hamsterObjects[number];
        hamster.SetActive(false);
        hamster.transform.position = Vector3.zero;
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
        HamsterPlay();
        if (/*myControllerInstance != null &&*/
           (myControllerInstance.isMulti && 
           !myControllerInstance.isHost)) return;
        if (hamsters.Count >= spawnHamsterCount) return;
        if (SpawnInterval()) SpawnHamster();
    }

    //撃破情報の送信用メソッド
    private void DestroyTransmission(int number)
    {
        if (!MagicOnionController.GetInstance.isHost) return;
        myControllerInstance.me.DestroyHamster(number);
    }

    //撃破時用メソッド
    private void DestroyHamster(Transform inTransform)
    {
        for(int i = 0; i < hamsters.Count; i++)
        {
            if (hamsters[i].gameObject != inTransform.gameObject) continue;
            DestroyTransmission(hamsters[i].GetComponent<Hamster>().number);
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
