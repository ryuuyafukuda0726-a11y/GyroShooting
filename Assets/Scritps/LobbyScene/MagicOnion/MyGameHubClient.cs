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

    //通信開始用メソッド
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
        await _client.JoinAsync(userId, name);
    }

    //通信切断用メソッド
    public async void LeaveClient(bool inHost)
    {
        await _client.LeaveAsync(inHost);
    }

    //ゲーム開始の通信用メソッド
    public async void GameStart()
    {
        await _client.GameStartAsync();
    }

    //待機状態の通信用メソッド
    public async void StayGameStart()
    {
        await _client.StayGameStartAsync();
    }

    //移動の情報を送信する
    public async void Move(Vector3 inPos, Quaternion inRot)
    {
        await _client.MoveAsync(inPos.ToMyVector3(), inRot.ToMyQuaternion());
    }

    //ハムスターの位置情報を送信する
    public async void HamsterDataTransmission(int count, MyVector3[] pos, MyQuaternion[] rot)
    {
        await _client.HamsterMoveAsync(count, pos, rot);
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