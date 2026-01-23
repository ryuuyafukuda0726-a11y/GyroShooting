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
    //ターゲティング用変数
    [SerializeField]
    private GameObject reticleImage;
    private Transform shotTarget;
    private bool isTarget = false;
    //入力用変数
    private TouchControl touch;
    private InputAction cameraAction;
    private float vert, horiz;
    private Vector2 tapPoint;
    [SerializeField]
    private float valueCorrection = 0.0f;
    //カメラの制御用変数
    private bool isControl = false;
    //private CinemachineFollow follow;
    private Vector3 targetPos;
    //プラットフォーム用変数
    private Platform myPlatformInstance;
    private float myTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraAction = GetComponent<PlayerInput>().actions["Look"];
        //follow = transform.GetChild(0).GetComponent<CinemachineFollow>();
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
        if (touch.press.wasPressedThisFrame && !isControl)
        {
            tapPoint = touch.position.ReadValue();
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = tapPoint;
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);
            isControl = results.Count <= 0 ? true : false;
        }
    }

    //入力用メソッド
    private void InputOperationValue()
    {
        if (touch.press.isPressed && isControl)
        {
            if (touch.position.ReadValue() == tapPoint) return;
            Vector2 value = (touch.position.ReadValue() - tapPoint);
            vert = value.y * valueCorrection;
            horiz = value.x * valueCorrection;
        }
    }

    //操作終了用メソッド
    private void OperationEnd()
    {
        if (touch.press.wasReleasedThisFrame && isControl)
        {
            isControl = false;
            vert = 0.0f;
            horiz = 0.0f;
        }
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
    //private void SetPos(Vector3 axis, float angle)
    //{
    //    Vector3 vector = follow.FollowOffset;
    //    Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
    //    Vector3 vector2 = vector - targetPos;
    //    vector2 = quaternion * vector2;
    //    vector = targetPos + vector2;
    //    follow.FollowOffset = vector;
    //}

    private void Wait()
    {
        myTime += Time.deltaTime;
        if(myTime > 1.0f)
        {
            myTime = 0.0f;
            Debug.Log(transform.rotation.x);
        }
    }

    //カメラ操作用メソッド
    private void CameraOperation()
    {
        Vector3 angle = new Vector3(horiz * 100.0f * Time.deltaTime, vert * 100.0f * Time.deltaTime, 0.0f);
        float x = transform.rotation.x;
        if (x > 25.0f / 360.0f && angle.y > 0.0f)
        {
            angle.y = 0.0f;
        }
        else if (x < -5.0f / 360.0f && angle.y < 0.0f)
        {
            angle.y = 0.0f;
        }
        transform.RotateAround(targetPos + Vector3.up * 2.0f, transform.up, angle.x);
        transform.RotateAround(targetPos + Vector3.up * 2.0f, transform.right, angle.y);
        Wait();
        Vector3 dis = (targetPos + Vector3.up * 2.0f) - transform.position;
        transform.rotation = Quaternion.LookRotation(dis);
    }

    //レティクル用メソッド
    private void SetReticleColor()
    {
        reticleImage.GetComponent<Image>().material.color =
            isTarget ? new Color(1.0f, 0.0f, 0.0f, 1.0f) :
                     new Color(1.0f, 0.0f, 0.0f, 0.5f);
    }

    //ターゲティング用メソッド
    private void Targeting()
    {
        shotTarget = null;
        isTarget = false;
        Camera camera = Camera.main;
        int centerX = camera.pixelWidth / 2;
        int centerY = camera.pixelHeight / 2;
        ray = camera.ScreenPointToRay(new Vector2(centerX, centerY));
        //Debug.DrawRay(ray.origin, transform.forward * 10.0f, Color.red, 5.0f);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if ((hit.collider.tag == "Hamster"))
            {
                shotTarget = hit.transform;
                isTarget = true;
            }
        }
        SetReticleColor();
    }

    //ターゲット取得用メソッド
    public Transform GetTarget()
    {
        return shotTarget;
    }

    //プレイ用メソッド
    public void Play(Vector3 inPos)
    {
        targetPos = inPos;
        if (!myPlatformInstance.CheckPlatform()) PCInputCameraOperation();
        else MobileInputCameraControl();
        CameraOperation();
        Targeting();
    }
}
