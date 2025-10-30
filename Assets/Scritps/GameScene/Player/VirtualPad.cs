using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Unity.VisualScripting;
using System;

//バーチャルパッド用変数
public class VirtualPad : MonoBehaviour
{
    //入力用変数
    TouchControl touch;
    Vector3 tapPoint;
    Vector3 distance;
    //パッド用変数
    [SerializeField]
    private GameObject padObject;
    private const float inputRange = 150.0f;
    //移動用変数
    private bool isMove = false;
    //モバイル操作のコールバック用変数
    [NonSerialized]
    public Action<Vector3> mobileControlCallBack;
    //プラットフォーム用変数
    private Platform myPlatformInsctance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnhancedTouchSupport.Enable();
        myPlatformInsctance = Platform.GetPlatformInstance;   
    }

    //入力地点の取得用メソッド
    private void GetTapPoint()
    {
        touch = Touchscreen.current.primaryTouch;
        tapPoint = touch.position.ReadValue();
        distance = tapPoint - transform.position;
    }

    //入力開始用メソッド
    private void InputStart()
    {
        if (isMove) return;
        if (!touch.press.wasPressedThisFrame) return;
        float value = Vector3.Distance(tapPoint, transform.position);
        if (value < inputRange) isMove = true;
    }

    //入力用メソッド
    private void Input()
    {
        if (!isMove) return;
        TouchControl touch = Touchscreen.current.primaryTouch;
        if (!touch.press.isPressed) return;
        float powor = Mathf.Min(2.0f, distance.magnitude / inputRange);
        Vector3 direction = distance.normalized;
        padObject.transform.localPosition = direction * inputRange / 2 * powor;
    }

    //入力終了用メソッド
    private void InputRelease()
    {
        if (!touch.press.wasReleasedThisFrame) return;
        isMove = false;
        padObject.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
    }

    //移動方向の生成用メソッド
    private Vector3 CreateMoveVec()
    {
        Vector3 padVec = padObject.GetComponent<RectTransform>().anchoredPosition3D;
        Vector3 moveVec = new Vector3(padVec.x, 0.0f, padVec.y);
        float value = Vector3.Distance(Vector3.zero, moveVec);
        return value > inputRange ? moveVec.normalized : moveVec / inputRange;
    }

    //プレイ用メソッド
    public void Play()
    {
        if (!myPlatformInsctance.CheckPlatform()) return;
        GetTapPoint();
        InputStart();
        Input();
        InputRelease();
        mobileControlCallBack(CreateMoveVec());
    }
}
