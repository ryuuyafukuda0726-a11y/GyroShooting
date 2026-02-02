using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;
using UnityEditor;
using System.Collections;
using System;

//ロビーシーンを管理するマネージャースクリプトクラス
public class LobbyManager : MonoBehaviour
{
    //名前用変数
    [SerializeField]
    private GameObject nameImage;
    private InputNameImage nameImageScript;
    private string playerName = "";
    [SerializeField]
    private int iDLength = 0;
    private string playerId = "";
    //アドレス用変数
    [SerializeField]
    private GameObject addressImage;
    private InputAddressImage addressImageScript;
    //プレイモード選択用変数
    [SerializeField]
    private GameObject playModeImage;
    private PlayModeImage playModeImageScript;
    //マッチング部屋用変数
    [SerializeField]
    private GameObject matchingRoomImage;
    private MatchingRoomImage matchingRoomScript;
    //シーン遷移時UI用変数
    [SerializeField]
    private GameObject sceneChangeUI;
    private SceneChangeUI sceneChangeUIScript;
    [SerializeField]
    private GameObject loadBar;
    //シーン遷移用変数
    private bool sceneChange = false;
    //進行管理用デリゲート変数
    private delegate void MyLobbyDelegate();
    private MyLobbyDelegate myLobbyDelegate;
    //プラットフォーム用変数
    private Platform myPlatformInstance;
    //通信用変数
    private MagicOnionController myControllerInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnhancedTouchSupport.Enable();
        nameImageScript = nameImage.GetComponent<InputNameImage>();
        addressImageScript = addressImage.GetComponent<InputAddressImage>();
        playModeImageScript = playModeImage.GetComponent<PlayModeImage>();
        matchingRoomScript = matchingRoomImage.GetComponent<MatchingRoomImage>();
        sceneChangeUIScript = sceneChangeUI.GetComponent<SceneChangeUI>();
        myPlatformInstance = Platform.GetPlatformInstance;
        myControllerInstance = MagicOnionController.GetInstance;
        myLobbyDelegate = Init;
    }

    //コールバックの設定用メソッド
    private void SetCallBack()
    {
        playModeImageScript.singleModeCallBack = CheckInput;
        playModeImageScript.multiModeCallBack = MultiModeCallBack;
        matchingRoomScript.returnButtonCallBack = ReturnButtonCallBack;
        matchingRoomScript.gameStartCallBack = GameStart;
    }

    //初期設定用メソッド
    private void Init()
    {
        nameImageScript.Init();
        addressImageScript.Init();
        playModeImageScript.Init();
        matchingRoomScript.Init();
        sceneChangeUIScript.Init();
        loadBar.transform.parent.gameObject.SetActive(false);
        SetCallBack();
        myLobbyDelegate = CheckExistingDataEasing;
    }

    //既存データ確認への遷移用メソッド用メソッド
    private void CheckExistingDataEasing()
    {
        if (!sceneChangeUIScript.EasingControl("Open")) return;
        myLobbyDelegate = CheckExistingData;
    }

    //既存データの確認用メソッド
    private void CheckExistingData()
    {
        if (PlayerPrefs.HasKey("PlayerID"))
        {
            playerId = PlayerPrefs.GetString("PlayerID");
            playerName = PlayerPrefs.GetString("PlayerName");
            myLobbyDelegate = LobbyEasing;
        }
        else myLobbyDelegate = InputNameDataEasing;
    }

    //名前入力へのイージング用メソッド
    private void InputNameDataEasing()
    {
        nameImage.SetActive(true);
        if (!nameImageScript.EasingControl("Open")) return;
        myLobbyDelegate = InputNameData;
    }

    //名前の入力用メソッド
    private void InputNameData()
    {
        nameImageScript.InputName();
        if (!nameImageScript.isEnd) return;
        Guid guid = new Guid();
        DateTimeOffset globalTimeOffset = DateTimeOffset.UtcNow;
        string ID = guid.ToString() + globalTimeOffset;
        PlayerPrefs.SetString("PlayerID", ID);
        PlayerPrefs.SetString("PlayerName", nameImageScript.GetName());
        myLobbyDelegate = LobbyEasing;
    }

    //ロビーへのイージング用メソッド
    private void LobbyEasing()
    {
        if (!playModeImageScript.EasingControl("Open")) return;
        myLobbyDelegate = Lobby;
    }

    //入力確認用メソッド
    private void CheckInput()
    {
        loadBar.transform.parent.gameObject.SetActive(true);
        myLobbyDelegate = ChangeGameScene;
    }

    //マルチの入力確認用メソッド
    private void MultiModeCallBack()
    {
        myControllerInstance.isMulti = true;
    }

    //ロビー用メソッド
    private void Lobby()
    {
        if (!myControllerInstance.isMulti) return;
        if (!playModeImageScript.EasingControl("Close")) return;
        myLobbyDelegate = InputAddressEasing;
    }

    //アドレスの入力へのイージング用メソッド
    private void InputAddressEasing()
    {
        addressImage.SetActive(true);
        if (!addressImageScript.EasingControl("Open")) return;
        myLobbyDelegate = InputAddress;
    }

    //アドレスの入力用メソッド
    private void InputAddress()
    {
        addressImageScript.InputAddress();
        if (!addressImageScript.isEnd) return;
        myControllerInstance.SetAddress(addressImageScript.address);
        myControllerInstance.JoinStart(playerId, playerName);
        myLobbyDelegate = MatchingRoomEasing;
    }

    //マッチングルームへのイージング用メソッド
    private void MatchingRoomEasing()
    {
        if (!matchingRoomScript.EasingControl("Open")) return;
        matchingRoomScript.GameStartButtonSetActive();
        myControllerInstance.receiver.OnGameStartCallBack = GameStart;
        matchingRoomScript.SetCallBack();
        myLobbyDelegate = MatchingRoom;
    }

    //マッチングルーム用メソッド
    private void MatchingRoom()
    {
        matchingRoomScript.Play();
    }

    //戻るボタンのコールバック用メソッド
    private void ReturnButtonCallBack()
    {
        myControllerInstance.isMulti = false;
        addressImageScript.Init();
        myControllerInstance.Leave();
        myLobbyDelegate = LobbyEasing;
    }

    //ゲーム開始時用メソッド
    public void GameStart()
    {
        if(myControllerInstance.isHost) myControllerInstance.me.GameStart();
        myLobbyDelegate = ChangeGameScene;
    }

    //シーン遷移のコルーチン呼び出し用メソッド
    private void LoadSceneAsync()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    //シーン遷移のコルーチン用メソッド
    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");

        while (!asyncLoad.isDone)
        {
            float value = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadBar.transform.localScale = new Vector3(value, 1.0f, 1.0f);
            Debug.Log("進行度 : " + value + "%");
            yield return null;
        }

        Debug.Log("遷移完了");
    }

    //ゲームシーンへの遷移用メソッド
    private void ChangeGameScene()
    {
        if (!sceneChangeUIScript.EasingControl("Close")) return;
        LoadSceneAsync();
    }

    // Update is called once per frame
    void Update()
    {
        myLobbyDelegate();
    }
}
