using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

//プレイヤーのカメラ用スクリプトクラス
public class PlayerCamera : MonoBehaviour
{
    //レイ用変数
    Ray ray;
    RaycastHit hit;
    [SerializeField]
    private GameObject canvas;
    private GraphicRaycaster raycaster;
    //入力用変数
    private TouchControl touch;
    private InputAction cameraAction;
    private float vert, horiz;
    private Vector2 tapPoint;
    [SerializeField]
    private float valueCorrection = 0.0f;
    //カメラの制御用変数
    private bool isControl = false;
    private CinemachineFollow follow;
    private Vector3 targetPos;
    //ターゲットの設定コールバック用変数
    public Action<Transform> SetTargetCallBack;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraAction = GetComponent<PlayerInput>().actions["Look"];
        follow = GetComponent<CinemachineFollow>();
        raycaster = canvas.GetComponent<GraphicRaycaster>();
        myPlatformInstance = Platform.GetPlatformInstance;
        EnhancedTouchSupport.Enable();
    }

    //PCでのカメラ操作入力用メソッド
    private void PCInputCameraOperation()
    {
        vert = -cameraAction.ReadValue<Vector2>().y;
        horiz = cameraAction.ReadValue<Vector2>().x;        
    }

    //操作開始用メソッド
    private void OperationStart()
    {
        if (!touch.press.wasPressedThisFrame || isControl) return;
        tapPoint = touch.position.ReadValue();
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = tapPoint;
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);               
        isControl = results.Count <= 0 ? true : false;
    }

    //入力用メソッド
    private void InputOperationValue()
    {
        if (!touch.press.isPressed || !isControl) return;
        //if (touch.position.ReadValue() == tapPoint) return;
        vert = -cameraAction.ReadValue<Vector2>().y;
        horiz = cameraAction.ReadValue<Vector2>().x;
        //Vector2 value = (touch.position.ReadValue() - tapPoint);
        //vert = value.y * valueCorrection;
        //horiz = value.x * valueCorrection;
        //Debug.Log(value);
    }

    //操作終了用メソッド
    private void OperationEnd()
    {
        if (!touch.press.wasReleasedThisFrame || !isControl) return;
        isControl = false;
        vert = 0.0f;
        horiz = 0.0f;
    }

    //モバイルでのカメラ操作入力用メソッド
    private void MobileInputCameraControl()
    {
        touch = Touchscreen.current.primaryTouch;
        OperationStart();
        InputOperationValue();
        OperationEnd();
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

    //カメラ操作用メソッド
    private void CameraOperation()
    {
        Vector3 angle = new Vector3(horiz * 100.0f * Time.deltaTime, vert * 100.0f * Time.deltaTime, 0.0f);
        if (transform.rotation.x > 0.5f && angle.y > 0.0f)
        {
            angle.y = 0.0f;
        }
        else if (transform.rotation.x < -0.5f && angle.y < 0.0f)
        {
            angle.y = 0.0f;
        }
        if (transform.rotation.y > 0.5f && angle.x > 0.0f)
        {
            angle.x = 0.0f;
        }
        else if (transform.rotation.y < -0.5f && angle.x < 0.0f)
        {
            angle.x = 0.0f;
        }
        SetPos(targetPos, Vector3.up, angle.x);
        SetPos(targetPos, Vector3.right, angle.y);
    }

    //エイム用メソッド
    private void Aim()
    {
        Camera camera = Camera.main;
        int centerX = camera.pixelWidth / 2;
        int centerY = camera.pixelHeight / 2;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        ray = camera.ScreenPointToRay(new Vector3(centerX, centerY, 0));
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;
        SetTargetCallBack(hit.collider.tag == "Humster" ? hit.collider.transform : null);
    }

    //プレイ用メソッド
    public void Play(Vector3 inPos)
    {
        targetPos = inPos;
        if (!myPlatformInstance.CheckPlatform()) PCInputCameraOperation();
        else MobileInputCameraControl();
        CameraOperation();
        Aim();
    }
}
