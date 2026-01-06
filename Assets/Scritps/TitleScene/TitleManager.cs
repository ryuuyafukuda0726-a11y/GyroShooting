using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;
using UnityEditor;
using System.Collections;

enum EasingControl
{
    SetEasing,
    Easing,
    EasingEnd
}

//タイトルシーンを管理するマネージャースクリプトクラス
public class TitleManager : MonoBehaviour
{
    //画面タッチの指示UI用変数
    [SerializeField]
    private GameObject touchScreenUI;
    private TouchScreenImage touchScreenScript;
    //シーン遷移時UI用変数
    [SerializeField]
    private GameObject sceneChangeUI;
    private SceneChangeUI sceneChangeUIScript;
    [SerializeField]
    private GameObject loadBar;
    //シーン遷移用変数
    private bool sceneChange = false;
    //進行管理用デリゲート変数
    private delegate void MyTitleDelegate();
    private MyTitleDelegate myTitleDelegate;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnhancedTouchSupport.Enable();
        myPlatformInstance = Platform.GetPlatformInstance;
        touchScreenScript = touchScreenUI.GetComponent<TouchScreenImage>();
        sceneChangeUIScript = sceneChangeUI.GetComponent<SceneChangeUI>();
        myTitleDelegate = Init;
        loadBar.transform.localScale = new Vector3(0.0f, 1.0f, 1.0f);
    }

    //初期設定用メソッド
    private void Init()
    {
        touchScreenScript.Init();
        sceneChangeUIScript.Init();
        myTitleDelegate = TitleEasing;
    }

    //タイトルへのイージング用メソッド
    private void TitleEasing()
    {
        if (!sceneChangeUIScript.EasingControl("Open")) return;
        myTitleDelegate = Title;
    }

    //PCでの入力確認用メソッド
    private void CheckInputPCPlatform()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        sceneChange = true;
    }

    //デバイスでの入力確認用メソッド
    private void CheckInputMobilePlatform()
    {
        TouchControl touch = Touchscreen.current.primaryTouch;
        if (!touch.press.wasPressedThisFrame) return;
        sceneChange = true;
    }

    //入力確認用メソッド
    private void doCheckInput()
    {
        if (!myPlatformInstance.CheckPlatform()) CheckInputPCPlatform();
        else CheckInputMobilePlatform();
    }

    //シーン遷移のコルーチン呼び出し用メソッド
    private void LoadSceneAsync()
    {
        StartCoroutine(LoadSceneCoroutine());
    }

    //シーン遷移のコルーチン用メソッド
    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LobbyScene");

        while (!asyncLoad.isDone)
        {
            float value = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadBar.transform.localScale = new Vector3(value, 1.0f, 1.0f);
            Debug.Log("進行度 : " + value + "%");
            yield return null;
        }

        Debug.Log("遷移完了");
    }

    //GameSceneへの遷移用メソッド
    private void ChengeGameScene()
    {
        if (!sceneChange) return;
        if (!sceneChangeUIScript.EasingControl("Close")) return;
        //SceneManager.LoadScene("GameScene");
        LoadSceneAsync();
    }

    //タイトル用メソッド
    private void Title()
    {
        ChengeGameScene();
        if (sceneChange) return;
        touchScreenScript.DisplayUI();
        doCheckInput();
    }

    // Update is called once per frame
    void Update()
    {
        myTitleDelegate();
    }
}
