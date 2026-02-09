using MagicOnionStudy.Shared;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//種を管理するマネージャースクリプトクラス
public class SeedManager
{
    //プレイヤー用変数
    private Transform player;
    private int damage = 0;
    private string iD = "";
    //種用変数
    private GameObject seedPrefab;
    private int maxSeed = 0;
    private int seedCount = 0;
    private int seed = 0;
    private int seedNumber = 0;
    private GameObject[] seedObjects;
    private GameObject seedParent;
    //発射用変数
    private Transform shotTransform;
    private const float rateTime = 0.5f;
    private float myTime = 0.0f;
    private bool isRate = false;
    ////ターゲット用変数
    //private bool isTarget = false;
    //private Vector3 targetVec;
    //private float correctionX = 0.0f;
    //コールバック用変数
    public Func<Transform> getTargetCallBack;
    public Action<int> shotCallBack;
    public Action<Vector3> setRotationCallBack;
    //通信用変数
    private MagicOnionController myConstrollerInstance;

    //種の生成用メソッド
    private void Seed()
    {
        seedParent = new GameObject("SeedParent" + iD);
        seedObjects = new GameObject[seedCount];
        for (int i = 0; i < seedCount; i++)
        {
            seedObjects[i] = GameObject.Instantiate(seedPrefab, seedParent.transform);
            seedObjects[i].SetActive(false);
        }
    }

    //初期設定用メソッド
    public void Init(Transform inTransform)
    {
        seed = maxSeed;
        Seed();
        shotTransform = inTransform;
        myTime = rateTime;
        isRate = false;
    }

    //発射レート管理用メソッド
    private void ShotRateControl()
    {
        if (!isRate) return;
        myTime += Time.deltaTime;
        if(myTime >= rateTime)
        {
            myTime = 0.0f;
            isRate = false;
        }
    }

    //発射用メソッド
    public void Shot(int inDamageValue)
    {
        //Debug.Log(isRate);
        if (seed <= 0) return;
        if (Mouse.current.leftButton.isPressed ||
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            setRotationCallBack(player.
                                GetChild(1).
                                GetComponent<PlayerCamera>().
                                CreateCharacterRotation());
            damage = inDamageValue;
            ShotRateControl();
            //SetTargetTransform(getTargetCallBack());
            ShotSeed();
        }
        ShotEnd();
    }

    //発射終了用メソッド
    private void ShotEnd()
    {
        if (!Mouse.current.leftButton.wasReleasedThisFrame) return;
        isRate = false;
        myTime = 0.0f;
    }

    //モバイルの発射用メソッド
    public void MobileShot(int inDamageValue)
    {
        if (seed <= 0) return;
        damage = inDamageValue;
        ShotRateControl();
        //SetTargetTransform(getTargetCallBack());
        ShotSeed();
        ShotEnd();
    }

    ////ターゲットの設定用メソッド
    //private void SetTargetTransform(Transform target)
    //{
    //    isTarget = false;
    //    if (target == null) return;
    //    isTarget = true;
    //    targetVec = (target.position - (player.position + Vector3.up * 0.5f)).normalized;
    //}

    //種の消滅情報を受信する
    public void ReceptionSeedDestroy(int number)
    {
        seedObjects[number].SetActive(false);
        seedObjects[number].transform.position = Vector3.zero;
    }

    //受信した種の情報を入力するメソッド
    public void ReceptionSeedDataInput(List<int> numbers,
                                       MyVector3[] position,
                                       MyQuaternion[] quaternion,
                                       int[] damage)
    {
        int size = numbers.Count;
        for(int i = 0; i < size; i++)
        {
            seedObjects[numbers[i]].SetActive(true);
            seedObjects[numbers[i]].transform.position = position[i].ToUnityVector3();
            seedObjects[numbers[i]].transform.rotation = quaternion[i].ToUnityQuaternion();
            seedObjects[numbers[i]].GetComponent<SunflowerSeed>().damage = damage[i];
        }
    }

    //情報を送信する種の選別用メソッド
    private List<int> CheckTransmissionSeedCount()
    {
        int size = seedObjects.Length;
        List<int> numbers = new List<int>();
        for(int i = 0; i < size; i++)
        {
            if (!seedObjects[i].activeSelf) continue;
            numbers.Add(i);
        }
        return numbers;
    }

    //種の情報を送信するメソッド
    public void SeedDataTransmission()
    {
        if (!myConstrollerInstance.isMulti) return;
        List<int> numbers = CheckTransmissionSeedCount();
        if (numbers.Count <= 0) return;
        MyVector3[] position = new MyVector3[numbers.Count];
        MyQuaternion[] quaternion = new MyQuaternion[numbers.Count];
        int[] damage = new int[numbers.Count];
        for(int i = 0; i < numbers.Count; i++)
        {
            position[i] = seedObjects[numbers[i]].transform.position.ToMyVector3();
            quaternion[i] = seedObjects[numbers[i]].transform.rotation.ToMyQuaternion();
            damage[i] = seedObjects[numbers[i]].GetComponent<SunflowerSeed>().damage;
        }
        myConstrollerInstance.me.SeedDataTransmission(numbers, position, quaternion, damage);
    }

    //種の発射用メソッド
    private void ShotSeed()
    {
        if (isRate) return;
        isRate = true;
        seedObjects[seedNumber].SetActive(true);
        seedObjects[seedNumber].transform.position = shotTransform.position;
        seedObjects[seedNumber].transform.GetChild(0).transform.rotation
            = shotTransform.rotation;
        Vector3 shotVec = shotTransform.forward;
        seedObjects[seedNumber].GetComponent<SunflowerSeed>().Shot(damage, shotVec, seedNumber);
        shotCallBack((int)GameSE.Shot);
        seedNumber++;
        seed--;
        //SeedDataTransmission();
        if (seedNumber < seedCount) return;
        seedNumber = 0;
    }

    //残弾補充用メソッド
    public void ChargeBullet()
    {
        if (seed >= maxSeed) return;
        seed++;
    }

    //残弾の取得用メソッド
    public int GetSeed()
    {
        return seed;
    }

    //コンストラクター
    public SeedManager(Transform inPlayer, GameObject inSeedPrefab,
                       int inMaxSeed, int inSeedCount, string inId)
    {
        myConstrollerInstance = MagicOnionController.GetInstance;
        player = inPlayer;
        seedPrefab = inSeedPrefab;
        maxSeed = inMaxSeed;
        seedCount = inSeedCount;
        iD = inId;
        //correctionX = inCorrection;
    }
}
