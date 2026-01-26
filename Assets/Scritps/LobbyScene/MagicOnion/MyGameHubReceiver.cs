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

    public Action<bool> OnJoinDelegate;

    public MyGameHubReceiver(Transform playerParent, GameObject playerPrefab, string selfId)
    {
        _playerParent = playerParent; _playerPrefab = playerPrefab; this.selfId = selfId;
    }
    public void OnJoin(string userId)
    {
        Debug.Log("Join Player:" + userId);

        if (selfId == userId)
        {            // 自分の場合は、既にいるので何もしないreturn;
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

    public void OnInitializeRoom(List<string> users)
    {
        foreach (var userId in users)
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
        throw new System.NotImplementedException();
    }
}