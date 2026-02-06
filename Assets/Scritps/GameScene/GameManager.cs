using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    //待機状態用変数
    private bool isStay = false;
    //キャンバス用変数
    [SerializeField]
    private Transform canvas;
    //タイマー用変数
    [SerializeField]
    private GameObject timer;
    private Timer timerScript;
    [SerializeField]
    private float time = 0.0f;
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
    [SerializeField]
    private Transform playerSpawnPoint;
    //マルチプレイヤー用変数
    [SerializeField]
    private Transform multiPlayerParent;
    private MultiPlayerParent multiPlayerParentScript;
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
    //クリア用変数
    private bool isClear = false;
    [SerializeField]
    private Result resultScript;
    //シーン遷移時UI用変数
    [SerializeField]
    private GameObject sceneChangeUI;
    private SceneChangeUI sceneChangeUIScript;
    //進行管理用デリゲート変数
    private delegate void MyGameDelegate();
    private MyGameDelegate myGameDelegate;
    //通信用変数
    private MagicOnionController myControllerInstance = null;
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
        timerScript = timer.GetComponent<Timer>();
        sunFlowerScript = sunFlower.GetComponent<SunFlower>();
        sunFlowerGageScript = sunFlowerGage.GetComponent<SunFlowerGage>();
        playerScript = playerObject.GetComponent<Player>();
        multiPlayerParentScript = multiPlayerParent.GetComponent<MultiPlayerParent>();
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
        sunFlowerScript.destroyCallBack = ResultEasing;
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
        resultScript.Init();
        timerScript.Init(time);
        ShotButtonInit();
        ItemInit();
        HamsterInit();
        playerScript.Init(audioSourceScript);
        multiPlayerParentScript.Init();
        lifeGageScript.Init();
        bulletGageScript.Init();
        sceneChangeUIScript.Init();
        sunFlowerGageScript.Init(sunFlowerScript.GetMaxHP());
        SetPlayerCallBack();
        SetMobileControlCallBack();
        SetSunFlowerCallBack();
        myGameDelegate = StayGameStart;
    }

    //ゲーム開始待機状態送信用メソッド
    private void StayGameStart()
    {
        if (/*myControllerInstance == null || */!myControllerInstance.isMulti)
        {
            SetPlayerRandomPos();
            myGameDelegate = InGameEasing;            
        }
        else/* if (myControllerInstance.isMulti)*/
        {
            if (isStay) return;
            myControllerInstance.receiver.otherPlayersParent = multiPlayerParent;
            Debug.Log("関数の登録前" + myControllerInstance.receiver.OnPlayStartCallBack);
            myControllerInstance.receiver.OnPlayStartCallBack = PlayStart;
            Debug.Log("関数の登録前" + myControllerInstance.receiver.OnPlayStartCallBack);
            myControllerInstance.me.StayGameStart();
            isStay = true;
        }
    }

    //プレイヤーをランダムに配置するメソッド
    private void SetPlayerRandomPos()
    {
        int size = 1;
        if (/*myControllerInstance != null &&*/ 
            myControllerInstance.isMulti) size += myControllerInstance.otherPlayers.Count;
        List<Transform> posList = new List<Transform>();
        for (int i = 0; i < playerSpawnPoint.childCount; i++)
        {
            posList.Add(playerSpawnPoint.GetChild(i));
        }
        for(int i = 0; i < size; i++)
        {
            int index = UnityEngine.Random.Range(0, posList.Count);
            if (i == 0) playerObject.transform.position = posList[index].position;
            else multiPlayerParent.GetChild(i -1).position = posList[index].position;
            posList.RemoveAt(index);
        }
    }

    //プレイ開始の受信時コールバック
    private void PlayStart()
    {
        isStay = false;
        SetPlayerRandomPos();
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
        if (timerScript.TimerCount()) ResultEasing(true);
        if (!myPlatformInstance.CheckPlatform()) return;
        virtualPadScript.Play();
    }

    //リザルトへのイージング用メソッド
    private void ResultEasing(bool inFlag)
    {
        isClear = inFlag;
        myGameDelegate = Result;
    }

    //結果表示
    private void Result()
    {
        if (!resultScript.ResultEasingControl(isClear)) return;
        myGameDelegate = End;
    }

    //終了メソッド
    private void End()
    {
        Debug.Log("終了");
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (/*myControllerInstance != null || */myControllerInstance.isMulti)
        {
            myControllerInstance.Leave();
        }
        SceneManager.LoadScene("TitleScene");
    }

    // Update is called once per frame
    void Update()
    {
        myGameDelegate();
    }
}
