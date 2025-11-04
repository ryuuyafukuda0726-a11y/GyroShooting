using UnityEngine;

//ゲームシーンを管理するマネージャースクリプトクラス
public class GameManager : MonoBehaviour
{
    //プレイヤー用変数
    [SerializeField]
    private GameObject playerObject;
    private Player playerScript;
    //バーチャルパッド用変数
    [SerializeField]
    private GameObject virtualPadObject;
    private VirtualPad virtualPadScript;
    //発射ボタン用変数
    [SerializeField]
    private GameObject shotButtonObject;
    //ライフ用変数
    [SerializeField]
    private GameObject lifeGageImage;
    private LifeGage lifeGageScript;
    //残弾用変数
    [SerializeField]
    private GameObject bulletGageImage;
    private BulletGage bulletGageScript;
    //シーン遷移時UI用変数
    [SerializeField]
    private GameObject sceneChangeUI;
    private SceneChangeUI sceneChangeUIScript;
    //進行管理用デリゲート変数
    private delegate void DoMyGameDelegate();
    private DoMyGameDelegate doMyGameDelegate;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myPlatformInstance = Platform.GetPlatformInstance;
        playerScript = playerObject.GetComponent<Player>();
        virtualPadScript = virtualPadObject.GetComponent<VirtualPad>();
        lifeGageScript = lifeGageImage.GetComponent<LifeGage>();
        bulletGageScript = bulletGageImage.GetComponent<BulletGage>();
        sceneChangeUIScript = sceneChangeUI.GetComponent<SceneChangeUI>();
        doMyGameDelegate = Init;
    }

    //モバイル操作のコールバック設定用メソッド
    private void SetMobileControlCallBack()
    {
        virtualPadScript.mobileControlCallBack = playerScript.MobileControlCallBack;
    }

    //PC操作時の初期設定用メソッド
    private void ShotButtonInit()
    {
        if (myPlatformInstance.CheckPlatform()) return;
        virtualPadObject.SetActive(false);
        shotButtonObject.SetActive(false);
    }

    //初期設定用メソッド
    private void Init()
    {
        SetMobileControlCallBack();
        ShotButtonInit();
        lifeGageScript.Init();
        bulletGageScript.Init();
        sceneChangeUIScript.Init();
        doMyGameDelegate = InGameEasing;
    }

    //インゲームへのイージング用メソッド
    private void InGameEasing()
    {
        if (!sceneChangeUIScript.EasingControl("Open")) return;
        doMyGameDelegate = InGame;
    }

    //インゲーム用メソッド
    private void InGame()
    {
        playerScript.Play();
        if (!myPlatformInstance.CheckPlatform()) return;
        virtualPadScript.Play();
    }

    // Update is called once per frame
    void Update()
    {
        doMyGameDelegate();
    }
}
