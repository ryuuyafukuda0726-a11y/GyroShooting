using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;
using UnityEditor;

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
    //シーン遷移用変数
    private bool sceneChange = false;
    //進行管理用デリゲート変数
    private delegate void DoMyTitleDelegate();
    private DoMyTitleDelegate doMyTitleDelegate;
    //プラットフォーム用変数
    private Platform myPlatformInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myPlatformInstance = Platform.GetPlatformInstance;
        touchScreenScript = touchScreenUI.GetComponent<TouchScreenImage>();
        sceneChangeUIScript = sceneChangeUI.GetComponent<SceneChangeUI>();
        doMyTitleDelegate = Init;
    }

    //初期設定用メソッド
    private void Init()
    {
        touchScreenScript.Init();
        sceneChangeUIScript.Init();
        doMyTitleDelegate = TitleEasing;
    }

    //タイトルへのイージング用メソッド
    private void TitleEasing()
    {
        if (!sceneChangeUIScript.EasingControl("Open")) return;
        doMyTitleDelegate = Title;
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

    //GameSceneへの遷移用メソッド
    private void ChengeGameScene()
    {
        if (!sceneChange) return;
        if (!sceneChangeUIScript.EasingControl("Close")) return;
        SceneManager.LoadScene("GameScene");
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
        doMyTitleDelegate();
    }
}
