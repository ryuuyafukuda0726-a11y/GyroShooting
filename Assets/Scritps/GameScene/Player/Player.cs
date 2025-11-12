using System;
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
    private const int maxLife = 10;
    private int life = maxLife;
    //種用変数
    [SerializeField]
    private GameObject seedPrefab;
    private const int maxSeed = 100;
    private int seed = maxSeed;
    //ターゲット用変数
    private bool isTarget = false;
    private Vector3 targetVec;
    private float correctionX = 0.0f;
    //カメラ用変数
    [SerializeField]
    private CinemachineCamera playerCamera;
    private PlayerCamera playerCameraScript;
    //コールバック用変数
    public Action<int> bulletGageDisplayCallBack;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    ////コールバックの設定用メソッド
    //private void SetCallBack()
    //{
    //    playerCameraScript.SetTargetCallBack = SetTargetTransform;
    //    playerCameraScript.ShotCallBack = CreateSeed;
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = GetComponent<PlayerInput>().actions["Move"];
        playerCameraScript = playerCamera.GetComponent<PlayerCamera>();
        //playerCameraScript.Init();
        correctionX = playerCamera.transform.rotation.x;
        //SetCallBack();
        myPlatformInstance = Platform.GetPlatformInstance;
    }

    //初期設定用メソッド
    public void Init()
    {
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
        cameraForward = Vector3.Scale(playerCamera.transform.forward, cameraCorrection);
        moveDirection = cameraForward * inputDirection.x 
            + playerCamera.transform.right * inputDirection.z;
    }

    //移動用メソッド
    private void Move()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    //発射用メソッド
    private void Shot()
    {
        if (seed <= 0) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        SetTargetTransform(playerCameraScript.GetTarget());
        CreateSeed();
        seed--;
    }

    //モバイルの発射用メソッド
    public void MobileShot()
    {
        if (seed <= 0) return;
        SetTargetTransform(playerCameraScript.GetTarget());
        CreateSeed();
        seed--;
    }

    //プレイ用メソッド
    public void Play()
    {
        bulletGageDisplayCallBack(seed);
        Input();
        Move();
        playerCameraScript.Play(transform.position);
        if (myPlatformInstance.CheckPlatform()) return;
        Shot();
    }

    //モバイル操作のコールバック用メソッド
    public void MobileControlCallBack(Vector3 inputVec)
    {
        moveDirection = inputVec;
    }

    //ターゲットの設定用メソッド
    private void SetTargetTransform(Transform target)
    {
        isTarget = false;
        if (target == null) return;
        isTarget = true;
        targetVec = (target.position - (transform.position + Vector3.up * 0.5f)).normalized;
    }

    //種の生成用メソッド
    private void CreateSeed()
    {
        Vector3 createPos = transform.position + Vector3.up * 0.5f;
        float rotX = Camera.main.transform.rotation.x - correctionX;
        float rotY = Camera.main.transform.rotation.y;
        float rotZ = Camera.main.transform.rotation.z;
        float rotW = Camera.main.transform.rotation.w;
        Quaternion createRot = isTarget ? Quaternion.LookRotation(targetVec) :
                                          new Quaternion(rotX, rotY, rotZ, rotW);
        GameObject.Instantiate(seedPrefab, createPos, createRot);
    }
}
