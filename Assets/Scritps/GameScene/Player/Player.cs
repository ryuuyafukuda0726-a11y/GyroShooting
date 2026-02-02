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
    //落下用変数
    [SerializeField]
    private float fallSpeed = 0.0f;
    private bool isFall = false;
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
    //接触判定用変数
    private Transform hitTransform;
    private Vector3 hitPos;
    //カメラ用変数
    [SerializeField]
    private GameObject playerCamera;
    private PlayerCamera playerCameraScript;
    //コールバック用変数
    public Action<int> bulletGageDisplayCallBack;
    public Action<int> lifeGageDisplayCallBack;
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
        myPlatformInstance = Platform.GetPlatformInstance;
    }

    //種の初期設定用メソッド
    private void SeedInit(GameAudioSource inAudioSource)
    {
        seedManagerScript = new SeedManager(transform,
                                            seedPrefab,
                                            maxSeed,
                                            seedCount,
                                            playerCamera.transform.rotation.x);
        seedManagerScript.Init(shotTransform);
        seedManagerScript.getTargetCallBack = playerCameraScript.GetTarget;
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

    //落下判定用メソッド
    private bool CheckFall()
    {
        if (hitTransform == null) isFall = true;
        else if (hitPos.y >= transform.position.y) isFall = true;
        else isFall = false;
        return isFall;
    }

    //落下用メソッド
    private void Fall()
    {
        if (!CheckFall()) return;
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
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

    //プレイ用メソッド
    public void Play()
    {
        bulletGageDisplayCallBack(seedManagerScript.GetSeed());
        lifeGageDisplayCallBack(life);
        Input();
        Move();
        InstallationTrap();
        //Fall();
        EfficacyControl();
        playerCameraScript.Play(transform.position);
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
    public void Damage(int inDamage)
    {
        if (life <= 0) return;
        life -= inDamage;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit");
        hitTransform = collision.transform;
        hitPos = collision.contacts[0].point;
    }

    //接触判定用メソッド
    private void OnCollisionStay(Collision collision)
    {
        
    }

    //
    private void OnCollisionExit(Collision collision)
    {
        hitTransform = null;
        isFall = true;
    }
}
