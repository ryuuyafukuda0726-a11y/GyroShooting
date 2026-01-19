using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;
using UnityEditor;
using System.Collections;

//ロビーシーンを管理するマネージャースクリプトクラス
public class LobbyManager : MonoBehaviour
{
    //名前UI用変数
    [SerializeField]
    private GameObject nameImage;
    private InputNameImage nameImageScript;
    private string playerName = "";
    //プレイモード選択UI用変数
    [SerializeField]
    private GameObject playModeImage;
    private PlayModeImage playModeImageScript;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnhancedTouchSupport.Enable();
        nameImageScript = nameImage.GetComponent<InputNameImage>();
        playModeImageScript = playModeImage.GetComponent<PlayModeImage>();
        sceneChangeUIScript = sceneChangeUI.GetComponent<SceneChangeUI>();
        myPlatformInstance = Platform.GetPlatformInstance;
        myLobbyDelegate = Init;
    }

    //初期設定用メソッド
    private void Init()
    {
        nameImageScript.Init();
        sceneChangeUIScript.Init();
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
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            playerName = PlayerPrefs.GetString("PlayerName");
            myLobbyDelegate = Lobby;
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
        myLobbyDelegate = LobbyEasing;
    }

    //ロビーへのイージング用メソッド
    private void LobbyEasing()
    {
        if (!playModeImageScript.EasingControl("Open")) return;
        myLobbyDelegate = Lobby;
    }

    //ロビー用メソッド
    private void Lobby()
    {
        Debug.Log("ようこそ");
    }

    // Update is called once per frame
    void Update()
    {
        myLobbyDelegate();
    }
}
