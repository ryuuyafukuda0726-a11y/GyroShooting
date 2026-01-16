using System;
using UnityEngine;
using UnityEngine.InputSystem;

//種を管理するマネージャースクリプトクラス
public class SeedManager
{
    //プレイヤー用変数
    private Transform player;
    //種用変数
    private GameObject seedPrefab;
    private int maxSeed = 0;
    private int seedCount = 0;
    private int seed = 0;
    private int seedNumber = 0;
    private GameObject[] seedObjects;
    private GameObject seedParent;
    //ターゲット用変数
    private bool isTarget = false;
    private Vector3 targetVec;
    private float correctionX = 0.0f;
    //コールバック用変数
    public Func<Transform> getTargetCallBack;
    public Action<int> shotCallBack;

    //種の生成用メソッド
    private void Seed()
    {
        seedParent = new GameObject("SeedParent");
        seedObjects = new GameObject[seedCount];
        for (int i = 0; i < seedCount; i++)
        {
            seedObjects[i] = GameObject.Instantiate(seedPrefab, seedParent.transform);
            seedObjects[i].SetActive(false);
        }
    }

    //初期設定用メソッド
    public void Init()
    {
        seed = maxSeed;
        Seed();
    }

    //発射用メソッド
    public void Shot()
    {
        if (seed <= 0) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        SetTargetTransform(getTargetCallBack());
        ShotSeed();
        seed--;
    }

    //モバイルの発射用メソッド
    public void MobileShot()
    {
        if (seed <= 0) return;
        SetTargetTransform(getTargetCallBack());
        ShotSeed();
        seed--;
    }

    //ターゲットの設定用メソッド
    private void SetTargetTransform(Transform target)
    {
        isTarget = false;
        if (target == null) return;
        isTarget = true;
        targetVec = (target.position - (player.position + Vector3.up * 0.5f)).normalized;
    }

    //種の発射用メソッド
    private void ShotSeed()
    {
        seedObjects[seedNumber].SetActive(true);
        Vector3 shotPos = player.position + Vector3.up * 0.5f;
        float rotX = Camera.main.transform.rotation.x /*- correctionX*/;
        float rotY = Camera.main.transform.rotation.y;
        float rotZ = Camera.main.transform.rotation.z;
        float rotW = Camera.main.transform.rotation.w;
        Quaternion shotRot = isTarget ? Quaternion.LookRotation(targetVec) :
                                          new Quaternion(rotX, rotY, rotZ, rotW);
        seedObjects[seedNumber].transform.position = shotPos;
        seedObjects[seedNumber].transform.rotation = shotRot;
        seedObjects[seedNumber].GetComponent<SunflowerSeed>().Shot();
        shotCallBack((int)GameSE.Shot);
        seedNumber++;
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
                       int inMaxSeed, int inSeedCount, float inCorrection)
    {
        player = inPlayer;
        seedPrefab = inSeedPrefab;
        maxSeed = inMaxSeed;
        seedCount = inSeedCount;
        correctionX = inCorrection;
    }
}
