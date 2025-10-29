using UnityEngine;

//プラットフォームの確認用スクリプトクラス
public class Platform : MonoBehaviour
{
    //プラットフォーム用変数
    private static Platform myPlatformInstance;
    //プラットフォームの判定用変数
    private bool mobile = false;

    //プラットフォームをDontDestroyOnLoadへの登録用メソッド
    private void SetDontDestroyPlatformInstance()
    {
        //インスタンスのDontDestroyOnLoadの登録状態を確認
        if (myPlatformInstance == null)
        {
            myPlatformInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //カーソルの設定用メソッド
    private void SetCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //起動時メソッド
    private void Awake()
    {
        SetDontDestroyPlatformInstance();
        mobile = UnityEngine.Device.Application.isMobilePlatform;
        if (!mobile) SetCursor();
    }

    //プラットフォームの確認用メソッド
    public bool CheckPlatform()
    {
        return mobile;
    }

    //プラットフォームインスタンスの取得用メソッド
    public static Platform GetPlatformInstance
    {
        get
        {
            return myPlatformInstance;
        }
    }
}
