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
    private const float inputRange = 200.0f;
    //移動用変数
    private bool isMove = false;
    //レイ用変数
    private Ray ray;
    private RaycastHit hit;
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
        if (!touch.press.wasPressedThisFrame) return;
        
    }

    //プレイ用メソッド
    public Vector3 doPlay()
    {
        Vector3 vec = padObject.GetComponent<RectTransform>().anchoredPosition3D;
        float value = Vector3.Distance(Vector3.zero, vec);
        return value > inputRange ? vec.normalized : vec / inputRange;
    }
}
