using System;
using System.Collections.Generic;
using MagicOnionStudy.Shared;
using UnityEngine;
public class MyGameHubReceiver : IMyGameHubReceiver
{
    private string selfId;
    private Dictionary<string, GameObject> _players = new();
    private Transform _playerParent;
    private GameObject _playerPrefab;
    //コールバック用変数
    public Action<bool> OnJoinDelegate;
    public Action OnCheckHostCallBack;
    public Action<float> OnTimerDisplayCallBack;
    //通信用変数
    private MagicOnionController myControllerInstance;

    public MyGameHubReceiver(Transform playerParent, GameObject playerPrefab, string selfId)
    {
        _playerParent = playerParent; _playerPrefab = playerPrefab; this.selfId = selfId;
        myControllerInstance = MagicOnionController.GetInstance;
    }

    public void OnJoin(string userId, string userName, bool inHost)
    {
        Debug.Log("Join Player:" + userName);

        if (selfId == userId)
        {            // 自分の場合は、既にいるので何もしないreturn;
            Debug.Log("ホスト : " + inHost);
            if (!inHost) return;
            OnCheckHostCallBack();
        }
        else {
            //SpawnOtherPlayer(userId); 
            OnJoinDelegate.Invoke(true);
        }
    }

    public void OnLeave(string userId)
    {
        Debug.Log("Leave Player:" + userId);

        if (_players.TryGetValue(userId, out GameObject player)) GameObject.Destroy(player);
    }

    public void OnInitializeRoom(List<string>[] users)
    {
        Debug.Log("部屋が初期化されました");
        foreach (var userId in users[1])
        {
            if (!_players.TryGetValue(userId, out GameObject player))
            {
                SpawnOtherPlayer(userId);
            }
        }
    }

    public void OnMove(string userId, MyVector3 position, MyQuaternion quaternion)
    {
        if (_players.TryGetValue(userId, out GameObject player))
        {
            if (player.name == selfId) return; // 自身の動きはサーバーから送られてくる情報を反映しなくてよい
            player.transform.SetPositionAndRotation(position.ToUnityVector3(), quaternion.ToUnityQuaternion());
        }
    }

    private void SpawnOtherPlayer(string userId)
    {
        Debug.Log("Spawn Player:" + userId);
        GameObject other = GameObject.Instantiate(_playerPrefab, _playerParent);
        other.name = userId;
        _players[userId] = other;
    }

    public void OnGameStart()
    {
        throw new System.NotImplementedException();
    }

    public void OnMatchingTimerCount(float time)
    {
        if (myControllerInstance.isHost) return;
        OnTimerDisplayCallBack(time);
    }
}