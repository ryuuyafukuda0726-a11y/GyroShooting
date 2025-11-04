using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

//プレイヤー用スクリプトクラス
public class Player : MonoBehaviour
{
    //
    private float correctionX;
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
    //種用変数
    [SerializeField]
    private GameObject seedPrefab;
    //カメラ用変数
    [SerializeField]
    private CinemachineCamera playerCamera;
    private PlayerCamera playerCameraScript;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = GetComponent<PlayerInput>().actions["Move"];
        playerCameraScript = playerCamera.GetComponent<PlayerCamera>();
        myPlatformInstance = Platform.GetPlatformInstance;
        correctionX = playerCamera.transform.rotation.x;
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

    //種の生成用メソッド
    public void CreateSeed()
    {
        Vector3 createPos = transform.position + Vector3.up * 0.5f;
        float x = playerCamera.transform.rotation.x;
        float y = playerCamera.transform.rotation.y;
        float z = playerCamera.transform.rotation.z;
        Quaternion createRot = Quaternion.Euler(x, y, z);
        GameObject.Instantiate(seedPrefab, createPos, playerCamera.transform.rotation);
    }

    //種の発射用メソッド
    private void ShotSeed()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        CreateSeed();
    }

    //プレイ用メソッド
    public void Play()
    {
        Input();
        Move();
        playerCameraScript.Play(transform.position);
        ShotSeed();
    }

    //モバイル操作のコールバック用メソッド
    public void MobileControlCallBack(Vector3 inputVec)
    {
        moveDirection = inputVec;
    }
}
