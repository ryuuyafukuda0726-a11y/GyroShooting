using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

//プレイヤー用スクリプトクラス
public class Player : MonoBehaviour
{
    //軸入力用変数
    private InputAction move;
    private Vector2 inputMoveAxis;
    private Vector3 inputDirection;
    //移動用変数
    private Vector3 cameraForward;
    private Vector3 moveDirection;
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
    //撃破時用変数
    [SerializeField]
    private float maxRefreshTime = 0.0f;
    private float refreshTime = 0.0f;
    private float healTime = 0.0f;
    private bool isDie = false;
    private bool isRefresh = false;
    //種用変数
    [SerializeField]
    private GameObject seedPrefab;
    [SerializeField]
    private int maxSeed = 0;
    [SerializeField]
    private int seedCount = 0;
    private SeedManager seedManagerScript;
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
    //カメラ用変数
    [SerializeField]
    private GameObject playerCamera;
    private PlayerCamera playerCameraScript;
    //コールバック用変数
    public Action<int> bulletGageDisplayCallBack;
    public Action<int> lifeGageDisplayCallBack;
    //通信用変数
    private MagicOnionController myControllerInstance;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = GetComponent<PlayerInput>().actions["Move"];
        playerCameraScript = playerCamera.GetComponent<PlayerCamera>();
        //playerCameraScript.Init();
        //correctionX = playerCamera.transform.rotation.x;
        //SetCallBack();
        moveSpeed = speed;
        life = maxLife;
        healTime = maxRefreshTime / maxLife;
        myPlatformInstance = Platform.GetPlatformInstance;
        myControllerInstance = MagicOnionController.GetInstance;
    }

    //種の初期設定用メソッド
    private void SeedInit(GameAudioSource inAudioSource)
    {
        seedManagerScript = new SeedManager(transform,
                                            seedPrefab,
                                            maxSeed,
                                            seedCount);
        seedManagerScript.Init(shotTransform);
        //seedManagerScript.getTargetCallBack = playerCameraScript.GetTarget;
        seedManagerScript.shotCallBack = inAudioSource.PlaySECallBack;
        seedManagerScript.setRotationCallBack = SetRotation;
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
        Vector3 cameraCorrection = new Vector3(1.0f, 0.0f, 1.0f).normalized;
        inputMoveAxis = move.ReadValue<Vector2>();
        inputDirection.z = inputMoveAxis.x;
        inputDirection.x = inputMoveAxis.y;
        cameraForward = Vector3.Scale(Camera.main.transform.forward, cameraCorrection);
        moveDirection = cameraForward * inputDirection.x 
            + Camera.main.transform.right * inputDirection.z;
    }

    //移動用メソッド
    private void Move()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        if (/*!CheckMulti()*/!myControllerInstance.isMulti) return;
        myControllerInstance.me.Move(transform.position, 
                                     transform.GetChild(0).rotation);
    }

    //プレイヤーの角度を操作するメソッド
    private void SetRotation(Vector3 vec)
    {
        transform.GetChild(0).rotation = Quaternion.Euler(vec);
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
        if(backTime > knockBackTime)
        {
            backTime = 0.0f;
            isKnockBack = false;
        }
        transform.Translate(knockBackVec * Time.deltaTime);
    }

    //時間経過での回復用メソッド
    private void TimeHeal()
    {
        if (isRefresh)
        {
                life++;
                isRefresh = false;
                Debug.Log("回復");
        }
    }

    //撃破時用メソッド
    private void Die()
    {
        if (!isDie) return;
        refreshTime += Time.deltaTime;
        if(refreshTime > maxRefreshTime)
        {
            refreshTime = 0.0f;
            isDie = false;
            isRefresh = false;
        }
        if (refreshTime % healTime <= 0.5f) isRefresh = true;
        else TimeHeal();
        Debug.Log((int)refreshTime % healTime);
    }

    //マルチの確認用メソッド
    private bool CheckMulti()
    {
        if (myControllerInstance != null)
        {
            if (myControllerInstance.isMulti) return true;
        }
        return false;
    }

    //プレイ用メソッド
    public void Play()
    {
        bulletGageDisplayCallBack(seedManagerScript.GetSeed());
        lifeGageDisplayCallBack(life);
        Die();
        playerCameraScript.Play(transform.position);
        if (isDie) return;
        Input();
        Move();

        KnockBack();
        InstallationTrap();
        EfficacyControl();
        if (myPlatformInstance.CheckPlatform()) return;
        seedManagerScript.Shot(damage);
    }

    //モバイル操作のコールバック用メソッド
    public void MobileControlCallBack(Vector3 inputVec)
    {
        moveDirection = inputVec;
    }

    //モバイル操作の発射用メソッド
    public void Shot()
    {
        seedManagerScript.MobileShot(damage);
    }

    //ダメージ用メソッド
    public void Damage(int inDamage, Vector3 inVec)
    {
        if (isDie) return;
        isKnockBack = true;
        knockBackVec = inVec;
        life -= inDamage;
        if (life <= 0) isDie = true;
    }

    //給弾用メソッド
    public void ChargeBullet()
    {
        seedManagerScript.ChargeBullet();
    }

    //アイテム取得時用メソッド
    public void SetItem(int inItemType)
    {
        itemType = (ItemType)inItemType;
        isItem = true;
        Debug.Log("アイテムゲット : " + (ItemType)inItemType);
    }
}
