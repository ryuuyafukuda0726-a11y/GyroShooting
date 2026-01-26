using System;
using Cysharp.Net.Http;
using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnionStudy.Shared;
using UnityEngine;
namespace MagicOnionStudy.Client.Scripts
{
    public class MagicOnionClientCreator : MonoBehaviour
    {
        //private GrpcChannel _channel;
        //public IMyFirstService Client { get; private set; }
        //[SerializeField]
        //private string _serverUrl = "http://127.0.0.1:5000";
        //async void Start()
        //{
        //    _channel = GrpcChannel.ForAddress(_serverUrl, new GrpcChannelOptions
        //    {
        //        HttpHandler = new YetAnotherHttpHandler()
        //        {
        //            Http2Only = true,
        //        },
        //        DisposeHttpClient = true,
        //    });
        //    // 2. サービスを呼び出すためのクライアントを生成
        //    Client = MagicOnionClient.Create<IMyFirstService>(_channel);
        //    // 3. 【動作確認用】実際にサーバーの SumAsync を呼び出してみる
        //    try
        //    {
        //        var result = await Client.SumAsync(123, 456);
        //        Debug.Log($"[MagicOnion] SumAsync(123, 456) の結果: {result}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError($"[MagicOnion] 通信エラーが発生しました: {ex.Message}");
        //    }
        //}
        void OnDestroy()
        {
            // チャンネルを適切にクローズする
            //_channel?.Dispose();
        }
    }
}
