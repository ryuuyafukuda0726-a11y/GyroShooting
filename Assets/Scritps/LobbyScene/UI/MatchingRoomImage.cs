using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

//マッチング部屋用スクリプトクラス
public class MatchingRoomImage : MonoBehaviour
{
    //UI用変数
    [SerializeField]
    private GameObject gameStartButton;
    //レクトトランスフォーム用変数
    private RectTransform rt;
    //イージング用変数
    private EasingControl easing;
    private Vector3 aVec, bVec;
    private float percent = 0.0f;
    private const float minPercent = 0.0f;
    private const float maxPercent = 1.0f;
    //マッチング用変数
    [SerializeField]
    private TextMeshProUGUI matchingTimeTMP;
    [SerializeField]
    private float matchingTime = 0.0f;
    private float timer = 0.0f;
    private bool isReturn = false;
    //マッチングプレイヤー用変数
    [SerializeField]
    private GameObject[] matchingPlayers = new GameObject[4];
    //プレイマップ用変数
    [SerializeField]
    private GameObject playMapImage;
    //通信用変数
    private MagicOnionController myControllerInstance;
    //コールバック用メソッド
    public Action gameStartCallBack;
    public Action returnButtonCallBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
        myControllerInstance = MagicOnionController.GetInstance;
    }

    //コールバックの設定用メソッド
    public void SetCallBack()
    {
        myControllerInstance.receiver.OnTimerDisplayCallBack = SetTimer;
        myControllerInstance.receiver.OnLeavePlayerCallBack = NameBannerInit;
    }

    //ネームバナーの初期設定用メソッド
    private void NameBannerInit()
    {
        int size = matchingPlayers.Length;
        for(int i = 0; i < size; i++)
        {
            matchingPlayers[i].transform.GetChild(0)
                              .GetComponent<TextMeshProUGUI>()
                              .text = "";
            matchingPlayers[i].SetActive(false);
        }
    }

    //初期設定用メソッド
    public void Init()
    {
        gameStartButton.SetActive(false);
        myControllerInstance.setStartButtonCallBack = GameStartButtonSetActive;
        NameBannerInit();
        rt.transform.localScale = Vector3.up;
        easing = global::EasingControl.SetEasing;
        timer = matchingTime;
    }

    //マップ写真と説明の表示用メソッド
    private void DisplayMapPictureAndExplanation()
    {

    }

    //ネームバナー表示用メソッド
    private void SetPlayerName(List<OtherPlayer> inOtherPlayer)
    {
        int size = 0;
        if (inOtherPlayer != null) size = inOtherPlayer.Count;
        for (int i = 0; i < size + 1; i++)
        {
            matchingPlayers[i].SetActive(true);
            TextMeshProUGUI tmp = matchingPlayers[i]
                .transform.GetChild(0)
                .GetComponent<TextMeshProUGUI>();
            if (i == 0) tmp.text = PlayerPrefs.GetString("PlayerName");
            else tmp.text = inOtherPlayer[i - 1].userName;
        }
    }

    ////プレイヤーの表示用メソッド
    //private void DisplayPlayer()
    //{
        
    //}

    //時間用メソッド
    private void TimerCount()
    {
        if (!myControllerInstance.isHost) return;
        timer -= Time.deltaTime;
        myControllerInstance.TimerCountTransmission(timer);
    }

    //受信した時間の取得用メソッド
    private void SetTimer(float inTime)
    {
        timer = inTime;
    }

    //時間表示用メソッド
    private void DisplayTimer()
    {
        matchingTimeTMP.text = "" + (int)timer;
    }

    //ゲームスタートボタンのアクティブ設定用メソッド
    public void GameStartButtonSetActive(bool inHost)
    {
        gameStartButton.SetActive(inHost);
    }

    //タイマーの終了時用メソッド
    private void TimerEnd()
    {
        if (timer > 0) return;
        gameStartCallBack();
    }

    //プレイ用メソッド
    public void Play()
    {
        Debug.Log("ホスト : " + myControllerInstance.isHost);
        DisplayTimer();
        SetPlayerName(myControllerInstance.GetOtherPlayer());
        Return();
        if (!myControllerInstance.isHost) return;
        TimerCount();
        TimerEnd();
    }

    //イージング設定用メソッド
    private void SetEasing(string inMove)
    {
        switch (inMove)
        {
            case "Open":
                aVec = Vector3.up;
                bVec = Vector3.one;
                break;
            case "Close":
                aVec = Vector3.one;
                bVec = Vector3.up;
                break;
            default:
                break;
        }
        percent = minPercent;
    }

    //イージング用メソッド
    private bool Easing()
    {
        percent += Time.deltaTime;
        rt.transform.localScale = Vector3.Lerp(aVec, bVec, percent);
        return percent >= maxPercent ? true : false;
    }

    //イージング管理用メソッド
    public bool EasingControl(string inMove)
    {
        //イージングの進行状態でスイッチ
        switch (easing)
        {
            case global::EasingControl.SetEasing:
                SetEasing(inMove);
                easing++;
                break;
            case global::EasingControl.Easing:
                //イージングの実行状態を確認
                if (Easing()) easing++;
                break;
            case global::EasingControl.EasingEnd:
                easing = global::EasingControl.SetEasing;
                return true;
            default:
                break;
        }
        return false;
    }

    //戻るボタン用メソッド
    public void ReturnButton()
    {
        isReturn = true;
    }

    //ひとつ前に戻るメソッド
    private void Return()
    {
        if (!isReturn) return;
        if (!EasingControl("Close")) return;
        isReturn = false;
        returnButtonCallBack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
