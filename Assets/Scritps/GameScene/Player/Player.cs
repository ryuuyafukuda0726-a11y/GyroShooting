using Fusion;
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
    //餌箱トラップ用変数
    [SerializeField]
    private GameObject feedingBoxPrefab;
    [SerializeField]
    private int maxTrap = 0;
    [SerializeField]
    private int trapCount = 0;
    private FeedingBoxManager feedingBoxManagerScript;
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
        life = maxLife;
        myPlatformInstance = Platform.GetPlatformInstance;
    }

    //種の初期設定用メソッド
    private void SeedInit()
    {
        seedManagerScript = new SeedManager(transform,
                                            seedPrefab,
                                            maxSeed,
                                            seedCount,
                                            playerCamera.transform.rotation.x);
        seedManagerScript.Init();
        seedManagerScript.getTargetCallBack = playerCameraScript.GetTarget;
    }

    //餌箱の初期設定用メソッド
    private void FeedingBoxInit()
    {
        feedingBoxManagerScript = new FeedingBoxManager(feedingBoxPrefab,
                                                        maxTrap,
                                                        trapCount);
        feedingBoxManagerScript.Init();
    }

    //トラップのリスト登録コールバックの設定用メソッド
    public void SetTrapListCallBack(Action<List<GameObject>> inAction)
    {
        feedingBoxManagerScript.setTrapListCallBack = inAction;
    }

    //初期設定用メソッド
    public void Init()
    {
        SeedInit();
        FeedingBoxInit();
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
        transform.Translate(moveDirection * speed * Time.deltaTime);
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
            feedingBoxManagerScript.CheckInstallationSpaceEnd(transform.position);
        }
        //feedingBoxManagerScript.InputInstallation(transform.position);
    }

    //プレイ用メソッド
    public void Play()
    {
        bulletGageDisplayCallBack(seedManagerScript.GetSeed());
        lifeGageDisplayCallBack(life);
        Input();
        Move();
        InstallationTrap();
        playerCameraScript.Play(transform.position);
        if (myPlatformInstance.CheckPlatform()) return;
        seedManagerScript.Shot();
    }

    //モバイル操作のコールバック用メソッド
    public void MobileControlCallBack(Vector3 inputVec)
    {
        moveDirection = inputVec;
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
}
