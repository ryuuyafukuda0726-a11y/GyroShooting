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
        sceneChangeUIScript = sceneChangeUI.GetComponent<SceneChangeUI>();
        doMyGameDelegate = Init;
    }

    //初期設定用メソッド
    private void Init()
    {
        virtualPadScript.Init();
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
        if (!myPlatformInstance.CheckPlatform()) playerScript.PCPlay();
        else playerScript.MobilePlay(virtualPadScript.Play());
    }

    // Update is called once per frame
    void Update()
    {
        doMyGameDelegate();
    }
}
