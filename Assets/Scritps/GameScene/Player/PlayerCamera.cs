using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

//プレイヤーのカメラ用スクリプトクラス
public class PlayerCamera : MonoBehaviour
{
    //入力用変数
    private InputAction cameraAction;
    private float vert, horiz;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraAction = GetComponent<PlayerInput>().actions["Look"];
        myPlatformInstance = Platform.GetPlatformInstance;
    }

    //PCでのカメラ操作用メソッド
    private void PCCameraControl(Vector3 inPos)
    {
        vert = cameraAction.ReadValue<Vector2>().x;
        horiz = cameraAction.ReadValue<Vector2>().y;
        Vector3 angle = new Vector3(horiz * 100.0f * Time.deltaTime, vert * 100.0f * Time.deltaTime, 0.0f);
        if(transform.rotation.x > 0.5f && angle.y > 0.0f)
        {
            angle.y = 0.0f;
        }
        else if(transform.rotation.x < -0.5f && angle.y < 0.0f)
        {
            angle.y = 0.0f;
        }
        transform.RotateAround(inPos, Vector3.up, angle.x);
        transform.RotateAround(inPos, Vector3.right, angle.y);
    }

    //モバイルでのカメラ操作用メソッド
    private void MobileCameraControl()
    {

    }

    //プレイ用メソッド
    public void Play(Vector3 inPos)
    {
        if (!myPlatformInstance.CheckPlatform()) PCCameraControl(inPos);
        else MobileCameraControl();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
