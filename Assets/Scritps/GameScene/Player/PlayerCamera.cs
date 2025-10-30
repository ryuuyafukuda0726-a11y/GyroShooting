using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

//プレイヤーのカメラ用スクリプトクラス
public class PlayerCamera : MonoBehaviour
{
    //レイ用変数
    private Ray ray;
    private RaycastHit hit;
    //入力用変数
    private TouchControl touch;
    private InputAction cameraAction;
    private float vert, horiz;
    private CinemachineFollow follow;
    private bool isControl = false;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraAction = GetComponent<PlayerInput>().actions["Look"];
        follow = GetComponent<CinemachineFollow>();
        myPlatformInstance = Platform.GetPlatformInstance;
        EnhancedTouchSupport.Enable();
    }

    //配置用メソッド
    private void SetPos(Vector3 point, Vector3 axis, float angle)
    {
        Vector3 vector = follow.FollowOffset;
        Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
        Vector3 vector2 = vector - point;
        vector2 = quaternion * vector2;
        vector = point + vector2;
        follow.FollowOffset = vector;
    }

    //PCでのカメラ操作用メソッド
    private void PCCameraControl(Vector3 inPos)
    {
        vert = -cameraAction.ReadValue<Vector2>().y;
        horiz = cameraAction.ReadValue<Vector2>().x;
        Vector3 angle = new Vector3(horiz * 100.0f * Time.deltaTime, vert * 100.0f * Time.deltaTime, 0.0f);
        if (transform.rotation.x > 0.5f && angle.y > 0.0f)
        {
            angle.y = 0.0f;
        }
        else if (transform.rotation.x < -0.5f && angle.y < 0.0f)
        {
            angle.y = 0.0f;
        }
        Debug.Log(angle);
        SetPos(inPos, Vector3.up, angle.x);
        SetPos(inPos, Vector3.right, angle.y);
    }

    //操作開始用メソッド
    private void ControlStart()
    {
        ray = Camera.main.ScreenPointToRay(touch.position.ReadValue());
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;
        if (hit.collider.tag == "UI") isControl = true;
    }

    //モバイルでのカメラ操作用メソッド
    private void MobileCameraControl()
    {
        touch = Touchscreen.current.primaryTouch;
        if (!touch.press.wasPressedThisFrame) return;
        ControlStart();
        
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
