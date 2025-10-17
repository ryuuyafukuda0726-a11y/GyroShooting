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
        myPlatformInstance = Platform.doGetPlatformInstance;
        playerScript = playerObject.GetComponent<Player>();
        virtualPadScript = virtualPadObject.GetComponent<VirtualPad>();
        sceneChangeUIScript = sceneChangeUI.GetComponent<SceneChangeUI>();
        doMyGameDelegate = doInit;
    }

    //初期設定用メソッド
    private void doInit()
    {
        virtualPadScript.doInit();
        sceneChangeUIScript.doInit();
        doMyGameDelegate = doInGameEasing;
    }

    //インゲームへのイージング用メソッド
    private void doInGameEasing()
    {
        if (!sceneChangeUIScript.doEasingControl("Open")) return;
        doMyGameDelegate = doInGame;
    }

    //インゲーム用メソッド
    private void doInGame()
    {
        if (!myPlatformInstance.doCheckPlatform()) playerScript.doPCPlay();
        else playerScript.doMobilePlay(virtualPadScript.doPlay());
    }

    // Update is called once per frame
    void Update()
    {
        doMyGameDelegate();
    }
}
