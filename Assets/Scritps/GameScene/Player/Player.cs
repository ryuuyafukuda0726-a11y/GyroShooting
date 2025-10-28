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
    //カメラ用変数
    [SerializeField]
    private CinemachineCamera playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        move = GetComponent<PlayerInput>().actions["Move"];
    }

    //入力用メソッド
    private void Input()
    {
        Vector3 cameraCorrection = new Vector3(1.0f, 0.0f, 1.0f);
        inputMoveAxis = move.ReadValue<Vector2>();
        inputDirection.z = inputMoveAxis.x;
        inputDirection.x = inputMoveAxis.y;
        cameraForward = Vector3.Scale(playerCamera.transform.forward, cameraCorrection);
        moveDirection = cameraForward * inputDirection.x + playerCamera.transform.right * inputDirection.z;
    }

    //移動用メソッド
    private void Move()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    //PCプレイ用メソッド
    public void PCPlay()
    {
        Input();
        Move();
    }

    //Mobileプレイ用メソッド
    public void MobilePlay(Vector3 inputVec)
    {
        moveDirection = inputVec;
        Move();
    }
}
