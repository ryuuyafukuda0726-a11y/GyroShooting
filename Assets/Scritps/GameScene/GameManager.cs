using UnityEngine;

//イージング管理
public enum EasingSequence
{
    SetEasing,
    Easing,
    EasingEnd
}

//ゲームシーンを管理するマネージャースクリプトクラス
public class GameManager : MonoBehaviour
{
    //キャンバス用変数
    [SerializeField]
    private Transform canvas;
    //AudioSource用変数
    [SerializeField]
    private GameObject audioSource;
    private GameAudioSource audioSourceScript;
    //ハムスター用変数
    [SerializeField]
    private GameObject hamsterPrefab;
    private HamusuterManager hamsterManagerScript;
    [SerializeField]
    private int spawnCount = 0;
    //プレイヤー用変数
    [SerializeField]
    private GameObject playerObject;
    private Player playerScript;
    //アイテム用変数
    private ItemManager itemManagerScript;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private int maxItemCount = 0;
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
    private delegate void MyGameDelegate();
    private MyGameDelegate myGameDelegate;
    //通信用変数
    private MagicOnionController myControllerInstance;
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
        myControllerInstance = MagicOnionController.GetInstance;
        sunFlowerScript = sunFlower.GetComponent<SunFlower>();
        sunFlowerGageScript = sunFlowerGage.GetComponent<SunFlowerGage>();
        playerScript = playerObject.GetComponent<Player>();
        //CreatePlayer();
        virtualPadScript = virtualPadObject.GetComponent<VirtualPad>();
        lifeGageScript = lifeGageImage.GetComponent<LifeGage>();
        bulletGageScript = bulletGageImage.GetComponent<BulletGage>();
        sceneChangeUIScript = sceneChangeUI.GetComponent<SceneChangeUI>();
        audioSourceScript = audioSource.GetComponent<GameAudioSource>();
        myGameDelegate = Init;
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
        playerScript.SetTrapListCallBack(hamsterManagerScript.SetTrapListCallBack);
    }

    //ひまわりのコールバック設定用メソッド
    private void SetSunFlowerCallBack()
    {
        sunFlowerScript.sunFlowerGageDisplayCallBack = sunFlowerGageScript.Display;
        sunFlowerScript.chargeBulletCallBack = playerScript.ChargeBullet;
    }

    //PC操作時の初期設定用メソッド
    private void ShotButtonInit()
    {
        if (myPlatformInstance.CheckPlatform()) return;
        virtualPadObject.SetActive(false);
        shotButtonObject.SetActive(false);
    }

    //アイテムの初期設定用メソッド
    private void ItemInit()
    {
        itemManagerScript = new ItemManager(maxItemCount, itemPrefab);
    }

    //ハムスターの初期設定用メソッド
    private void HamsterInit()
    {
        hamsterManagerScript = new HamusuterManager(sunFlower, 
                                                    spawnCount, 
                                                    hamsterPrefab, 
                                                    playerObject.transform, 
                                                    spawnPoint,
                                                    canvas);
        hamsterManagerScript.Init();
        hamsterManagerScript.SetCallBack(itemManagerScript.ItemDropCallBack);
    }

    //初期設定用メソッド
    private void Init()
    {
        ShotButtonInit();
        ItemInit();
        HamsterInit();
        playerScript.Init(audioSourceScript);
        lifeGageScript.Init();
        bulletGageScript.Init();
        sceneChangeUIScript.Init();
        sunFlowerGageScript.Init(sunFlowerScript.GetMaxHP());
        SetPlayerCallBack();
        SetMobileControlCallBack();
        SetSunFlowerCallBack();
        myGameDelegate = StayGameStart;
    }

    //ゲーム開始待機状態用メソッド
    private void StayGameStart()
    {
        if (myControllerInstance.isMulti)
        {
            myControllerInstance.receiver.OnPlayStartCallBack = PlayStart;
            myControllerInstance.me.StayGameStart();
        }
        else myGameDelegate = InGameEasing;
    }

    //プレイ開始の受信時コールバック
    private void PlayStart()
    {
        myGameDelegate = InGameEasing;
    }

    //インゲームへのイージング用メソッド
    private void InGameEasing()
    {
        if (!sceneChangeUIScript.EasingControl("Open")) return;
        audioSourceScript.PlayBGM((int)GameBGM.Main);
        myGameDelegate = InGame;
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
        myGameDelegate();
    }
}
