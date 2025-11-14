using UnityEngine;

//ゲームシーンを管理するマネージャースクリプトクラス
public class GameManager : MonoBehaviour
{
    //ハムスター用変数
    [SerializeField]
    private GameObject hamsterPrefab;
    private HamusuterManager hamsterManagerScript;
    [SerializeField]
    private int spawnCount = 0;
    //プレイヤー用変数
    [SerializeField]
    //private GameObject playerPrefab;
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
    //スポーン地点用変数
    [SerializeField]
    private Transform spawnPoint;
    //ひまわり用変数
    [SerializeField]
    private Transform sunFlower;
    private SunFlower sunFlowerScript;
    [SerializeField]
    private GameObject sunFlowerGage;
    private SunFlowerGage sunFlowerGageScript;
    //シーン遷移時UI用変数
    [SerializeField]
    private GameObject sceneChangeUI;
    private SceneChangeUI sceneChangeUIScript;
    //進行管理用デリゲート変数
    private delegate void DoMyGameDelegate();
    private DoMyGameDelegate doMyGameDelegate;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    //プレイヤーの生成
    private void CreatePlayer()
    {
        //playerObject = GameObject.Instantiate(playerPrefab);
        playerScript = playerObject.GetComponent<Player>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myPlatformInstance = Platform.GetPlatformInstance;
        sunFlowerScript = sunFlower.GetComponent<SunFlower>();
        sunFlowerGageScript = sunFlowerGage.GetComponent<SunFlowerGage>();
        playerScript = playerObject.GetComponent<Player>();
        //CreatePlayer();
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

    //プレイヤーのコールバック設定用メソッド
    private void SetPlayerCallBack()
    {
        playerScript.bulletGageDisplayCallBack = bulletGageScript.Display;
        playerScript.lifeGageDisplayCallBack = lifeGageScript.Display;
    }

    //ひまわりのコールバック設定用メソッド
    private void SetSunFlowerCallBack()
    {
        sunFlowerScript.sunFlowerGageDisplayCallBack = sunFlowerGageScript.Display;
    }

    //PC操作時の初期設定用メソッド
    private void ShotButtonInit()
    {
        if (myPlatformInstance.CheckPlatform()) return;
        virtualPadObject.SetActive(false);
        shotButtonObject.SetActive(false);
    }

    //ハムスターの初期設定用メソッド
    private void HamsterInit()
    {
        hamsterManagerScript = new HamusuterManager(sunFlower, 
                                                    spawnCount, 
                                                    hamsterPrefab, 
                                                    playerObject.transform, 
                                                    spawnPoint);
        hamsterManagerScript.Init();
    }

    //初期設定用メソッド
    private void Init()
    {
        SetPlayerCallBack();
        SetMobileControlCallBack();
        SetSunFlowerCallBack();
        ShotButtonInit();
        HamsterInit();
        //playerScript.Init();
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
        sunFlowerScript.Play();
        playerScript.Play();
        hamsterManagerScript.Play();
        if (!myPlatformInstance.CheckPlatform()) return;
        virtualPadScript.Play();
    }

    // Update is called once per frame
    void Update()
    {
        doMyGameDelegate();
    }
}
