using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

//バーチャルパッド用変数
public class VirtualPad : MonoBehaviour
{
    //パッド用変数
    [SerializeField]
    private GameObject padObject;
    private const float inputRange = 1.0f;
    //プラットフォーム用変数
    private Platform myPlatformInsctance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myPlatformInsctance = Platform.doGetPlatformInstance;   
    }

    //初期設定用メソッド
    public void doInit()
    {
        if (myPlatformInsctance.doCheckPlatform()) return;
        transform.gameObject.SetActive(false);
    }

    //入力用メソッド
    private void doInput()
    {
        TouchControl touch = Touchscreen.current.primaryTouch;
        
    }

    //プレイ用メソッド
    public Vector3 doPlay()
    {
        Vector3 padVec = padObject.GetComponent<RectTransform>().anchoredPosition3D;
        Vector3 virtualPadVec = GetComponent<RectTransform>().anchoredPosition3D;
        Vector3 vec = padVec - virtualPadVec;
        return vec.normalized;
    }
}
