using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

//プレイヤー用スクリプトクラス
public class MultiPlayer : MonoBehaviour
{
    //プレイヤー情報用変数
    [SerializeField]
    public OtherPlayer myData;
    //移動用変数
    private bool isKnockBack = false;
    private Vector3 knockBackVec;
    private const float knockBackTime = 0.25f;
    private float backTime = 0.0f;
    //移動速度用変数
    [SerializeField]
    private float speed = 0.0f;
    private float moveSpeed = 0.0f;
    //ライフ用変数
    [SerializeField]
    private int maxLife = 0;
    private int life = 0;
    //種用変数
    [SerializeField]
    private GameObject seedPrefab;
    [SerializeField]
    private int maxSeed = 0;
    [SerializeField]
    private int seedCount = 0;
    [NonSerialized]
    public SeedManager seedManagerScript;
    [SerializeField]
    private int damageValue = 0;
    private int damage = 0;
    //発射地点用変数
    [SerializeField]
    private Transform shotTransform;
    //餌箱トラップ用変数
    [SerializeField]
    private GameObject feedingBoxPrefab;
    [SerializeField]
    private int maxTrap = 0;
    [SerializeField]
    private int trapCount = 0;
    [SerializeField]
    private int seedCost = 0;
    private FeedingBoxManager feedingBoxManagerScript;
    //アイテム用変数
    private bool isItem = false;
    private ItemType itemType = 0;
    [SerializeField]
    private float itemEfficacyTime = 0.0f;
    private float myTime = 0.0f;
    [SerializeField]
    private float speedUpValue = 0.0f;
    [SerializeField]
    private int bulletSupplyValue = 0;
    [SerializeField]
    private float powerUpValue = 0.0f;
    [SerializeField]
    private int recoveryValue = 0;
    //コールバック用変数
    public Action<int> bulletGageDisplayCallBack;
    public Action<int> lifeGageDisplayCallBack;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerCameraScript.Init();
        //correctionX = playerCamera.transform.rotation.x;
        //SetCallBack();
        moveSpeed = speed;
        life = maxLife;
        myPlatformInstance = Platform.GetPlatformInstance;
    }

    //種の初期設定用メソッド
    private void SeedInit(GameAudioSource inAudioSource)
    {
        seedManagerScript = new SeedManager(transform,
                                            seedPrefab,
                                            maxSeed,
                                            seedCount);
        seedManagerScript.Init(shotTransform);
        seedManagerScript.shotCallBack = inAudioSource.PlaySECallBack;
    }

    //餌箱の初期設定用メソッド
    private void FeedingBoxInit()
    {
        feedingBoxManagerScript = new FeedingBoxManager(feedingBoxPrefab,
                                                        maxTrap,
                                                        trapCount);
        feedingBoxManagerScript.Init();
        feedingBoxManagerScript.setTrapCallBack = SetTrapCallBack;
    }

    //トラップのリスト登録コールバックの設定用メソッド
    public void SetTrapListCallBack(Action<List<GameObject>> inAction)
    {
        feedingBoxManagerScript.setTrapListCallBack = inAction;
    }

    //初期設定用メソッド
    public void Init(GameAudioSource inAudioSource)
    {
        SeedInit(inAudioSource);
        FeedingBoxInit();
        damage = damageValue;

        //move = GetComponent<PlayerInput>().actions["Move"];
        //playerCameraScript = playerCamera.GetComponent<PlayerCamera>();
        //playerCameraScript.Init();
        //correctionX = playerCamera.transform.rotation.x;
        ////SetCallBack();
        //myPlatformInstance = Platform.GetPlatformInstance;
    }

    //入力用メソッド
    private void Input()
    {
        if (myPlatformInstance.CheckPlatform()) return;
    }

    //移動用メソッド
    private void Move()
    {

    }

    //プレイヤーの角度を操作するメソッド
    private void SetRotation(float inXRot)
    {
        Vector3 a = transform.GetChild(0).rotation.eulerAngles;
        a.x = inXRot;
        transform.GetChild(0).rotation = Quaternion.Euler(a);
    }

    //餌箱の設置用メソッド
    private void InstallationTrap()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            feedingBoxManagerScript.InputInstallation();
        }
        if (Keyboard.current.tKey.isPressed)
        {
            feedingBoxManagerScript.CheckInstallationSpace(transform.position);
        }
        if (Keyboard.current.tKey.wasReleasedThisFrame)
        {
            // if()
            bool isCost = seedCount >= seedCost ? true : false;
            feedingBoxManagerScript.CheckInstallationSpaceEnd(transform.position, isCost);
        }
        //feedingBoxManagerScript.InputInstallation(transform.position);
    }

    //餌箱設置時のコールバック用メソッド
    private void SetTrapCallBack()
    {
        seedCount -= seedCost;
    }

    //アイテム使用後プロパティリセット用メソッド
    private void AfterUseItem()
    {
        moveSpeed = speed;
        damage = damageValue;
        myTime = 0.0f;
        isItem = false;
    }

    //アイテムの効果時間管理用メソッド
    private void EfficacyTimeControl()
    {
        myTime += Time.deltaTime;
        if (myTime > itemEfficacyTime)
        {
            AfterUseItem();
        }
    }

    //アイテムの効果用メソッド
    private void ItemEfficacy()
    {
        switch (itemType)
        {
            case ItemType.SpeedUp:
                moveSpeed = speed * 1.5f;
                break;
            case ItemType.BulletSupply:
                seedCount += bulletSupplyValue;
                if (seedCount > maxSeed) seedCount = maxSeed;
                break;
            case ItemType.Shooting:
                damage = (int)(damageValue * powerUpValue);
                break;
            case ItemType.Recovery:
                life += recoveryValue;
                if (life > maxLife) life = maxLife;
                AfterUseItem();
                break;
            default:
                break;
        }
    }

    //アイテムの効果管理用メソッド
    private void EfficacyControl()
    {
        if (!isItem) return;
        EfficacyTimeControl();
        ItemEfficacy();
    }

    //ノックバック用メソッド
    private void KnockBack()
    {
        if (!isKnockBack) return;
        backTime += Time.deltaTime;
        if (backTime > knockBackTime)
        {
            backTime = 0.0f;
            isKnockBack = false;
        }
        transform.Translate(knockBackVec /* 0.25f*/ * Time.deltaTime);
    }

    //プレイ用メソッド
    public void Play()
    {
        //bulletGageDisplayCallBack(seedManagerScript.GetSeed());
        //lifeGageDisplayCallBack(life);
        //Input();
        //Move();
        //KnockBack();
        //InstallationTrap();
        //EfficacyControl();
        //if (myPlatformInstance.CheckPlatform()) return;
        //seedManagerScript.Shot(damage);
    }

    //モバイル操作のコールバック用メソッド
    //public void MobileControlCallBack(Vector3 inputVec)
    //{
    //    moveDirection = inputVec;
    //}

    ////モバイル操作の発射用メソッド
    //public void Shot()
    //{
    //    seedManagerScript.MobileShot(damage);
    //}

    //ダメージ用メソッド
    public void Damage(int inDamage, Vector3 inVec)
    {
        //isKnockBack = true;
        //knockBackVec = inVec;
        //if (life <= 0) return;
        //life -= inDamage;
    }

    //給弾用メソッド
    public void ChargeBullet()
    {
        //seedManagerScript.ChargeBullet();
    }

    //アイテム取得時用メソッド
    public void SetItem(int inItemType)
    {
        //itemType = (ItemType)inItemType;
        //isItem = true;
    }
}
