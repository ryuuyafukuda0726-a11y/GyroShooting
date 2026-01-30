using System;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.AudioSettings;

public class MagicOnionController : MonoBehaviour
{
    //
    private static MagicOnionController myControllerInstance;
    //通信用変数
    [Header("Self Information")]
    [SerializeField] private string _selfId;
    [SerializeField] private string _selfName;
    [Header("Network Information")]
    [SerializeField] private string _serverUrl = "http://127.0.0.1:5000";
    [DoNotSerialize] public MyGameHubReceiver receiver;
    [Header("Players Base")]
    [SerializeField] private Transform _playerParent;
    [SerializeField] private MyGameHubClient _myPrefab;
    [SerializeField] private GameObject _otherPlayerPrefab;
    public MyGameHubClient me = null;
    [NonSerialized] public bool isHost = false;
    //その他のプレイヤー用変数
    private List<string> otherPlayerName;

    // MagicOnionController.me.client.XXAsync()

    // MagicOnionController.receiver.OnJoinDelegate

    //コールバックの設定用メソッド
    private void SetCallBack()
    {
        receiver.OnJoinDelegate += IAmJoin;//既定の名称の関数を登録
        receiver.OnJoinDelegate += flag => { Debug.Log("I am Join! : " + flag); }; //匿名関数の登録
        receiver.OnCheckHostCallBack = () => { isHost = true; };
    }

    //通信開始用メソッド
    public void JoinStart(string userId, string userName)
    {
        _selfId = userId;
        _selfName = userName;
        receiver = new MyGameHubReceiver(_playerParent, _otherPlayerPrefab, _selfId);
        SetCallBack();
        me = new MyGameHubClient();
        me.InitializeClient(_serverUrl, _selfId, _selfName, receiver);
    }

    //タイマー情報の送信用メソッド
    public void TimerCountTransmission(float inTime)
    {
        me._client.MatchingTimerCountAsync(inTime);
    }

    //通信中のプレイヤーを登録するメソッド
    public void SetCommunicatingPlayer(string inName)
    {
        otherPlayerName.Add(inName);
    }

    //他プレイヤーの名前取得用メソッド
    public List<string> GetOtherPlayerName()
    {
        return otherPlayerName;
    }

    //起動時用メソッド
    private void Awake()
    {        
        SetDontDestroyPlatformInstance();
    }

    public void IAmJoin(bool flag)
    {
        Debug.Log("I am Join! : " + flag);
    }

    //プラットフォームをDontDestroyOnLoadへの登録用メソッド
    private void SetDontDestroyPlatformInstance()
    {
        //インスタンスのDontDestroyOnLoadの登録状態を確認
        if (myControllerInstance == null)
        {
            myControllerInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //インスタンスの取得用メソッド
    public static MagicOnionController GetInstance
    {
        get
        {
            return myControllerInstance;
        }
    }
}