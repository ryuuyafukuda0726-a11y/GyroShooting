using System;
using Cysharp.Net.Http;
using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnionStudy.Shared;
using UnityEngine;
using UnityEngine.InputSystem;
public class MyGameHubClient
{
    private GrpcChannel _channel;
    public IMyGameHub _client;
    private string _serverUrl;
    public string userId;
    private string name;

    public async void InitializeClient(string serverUrl, string userId, string name, IMyGameHubReceiver receiver)
    {
        this._serverUrl = serverUrl;
        this.userId = userId;
        this.name = name;
        _channel = GrpcChannel.ForAddress(_serverUrl, new GrpcChannelOptions
        {
            HttpHandler = new YetAnotherHttpHandler()
            {
                Http2Only = true,
                Http2KeepAliveInterval = TimeSpan.FromSeconds(15),
                Http2KeepAliveTimeout = TimeSpan.FromSeconds(5)
            },
            DisposeHttpClient = true,
        }); 
        _client = await StreamingHubClient.ConnectAsync<IMyGameHub, IMyGameHubReceiver>(_channel, receiver);
    }

    //マッチング時間の通信用メソッド
    public async void OnMatchingTimerCount(float inTime)
    {
        await _client.MatchingTimerCountAsync(inTime);
    }

    //async void Update()
    //{
    //    try
    //    {
    //        // 以下はプレイヤーの操作によって自分の分身を動かす処理。採用しているプロジェクトによって適宜変更を推奨。
    //        if (Keyboard.current.upArrowKey.isPressed)
    //        {
    //            transform.position += Vector3.up * Time.deltaTime;
    //            var myPos = transform.position.ToMyVector3();
    //            var myRot = transform.rotation.ToMyQuaternion();
    //            await _client.MoveAsync(myPos, myRot);
    //        }

    //        if (Keyboard.current.downArrowKey.isPressed)
    //        {
    //            transform.position += Vector3.down * Time.deltaTime;
    //            var myPos = transform.position.ToMyVector3();
    //            var myRot = transform.rotation.ToMyQuaternion();
    //            await _client.MoveAsync(myPos, myRot);
    //        }

    //        if (Keyboard.current.rightArrowKey.isPressed)
    //        {
    //            transform.position += Vector3.right * Time.deltaTime;
    //            var myPos = transform.position.ToMyVector3();
    //            var myRot = transform.rotation.ToMyQuaternion();
    //            await _client.MoveAsync(myPos, myRot);
    //        }

    //        if (Keyboard.current.leftArrowKey.isPressed)
    //        {
    //            transform.position += Vector3.left * Time.deltaTime;
    //            var myPos = transform.position.ToMyVector3();
    //            var myRot = transform.rotation.ToMyQuaternion();
    //            await _client.MoveAsync(myPos, myRot);
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError($"[MagicOnion] 通信エラーが発生しました: {e.Message}");
    //    }
    //}
}