using System;
using System.Collections.Generic;
using MagicOnionStudy.Shared;
using UnityEngine;
using UnityEngine.UIElements;
public class MyGameHubReceiver : IMyGameHubReceiver
{
    private string selfId;
    public Transform otherPlayersParent;
    private Dictionary<string, GameObject> _players = new();
    private Transform _playerParent;
    private GameObject _playerPrefab;
    //コールバック用変数
    //public Action<bool> OnJoinDelegate;
    public Action<bool> OnCheckHostCallBack;
    public Action<float> OnTimerDisplayCallBack;
    public Action OnGameStartCallBack;
    public Action OnLeavePlayerCallBack;
    public Action OnPlayStartCallBack;
    public Action<int, MyVector3[], MyQuaternion[]> OnHamsterMoveCallBack;
    public Action<int> OnHamsterDestroyCallBack;
    //通信用変数
    private MagicOnionController myControllerInstance;

    public MyGameHubReceiver(Transform playerParent, GameObject playerPrefab, string selfId)
    {
        _playerParent = playerParent; 
        _playerPrefab = playerPrefab; 
        this.selfId = selfId;
        myControllerInstance = MagicOnionController.GetInstance;
    }

    public void OnJoin(string userId, string userName, bool inHost)
    {
        Debug.Log("Join Player:" + userName);
        if (selfId == userId)
        {            // 自分の場合は、既にいるので何もしないreturn;
            OnCheckHostCallBack(inHost);
        }
        else
        {
            myControllerInstance.SetJoinPlayer(userId, userName);
            //SpawnOtherPlayer(userId); 
            //OnJoinDelegate.Invoke(true);
        }
    }

    public void OnLeave(string userId)
    {
        Debug.Log("Leave Player:" + userId);
        myControllerInstance.DeleteLeavePlayer(userId);
        OnLeavePlayerCallBack();
        if (_players.TryGetValue(userId, out GameObject player)) GameObject.Destroy(player);
    }

    public void OnInitializeRoom(List<MyPlayerData> users)
    {
        Debug.Log("部屋が初期化されました");
        myControllerInstance.SetCommunicatingPlayer(users);

        //myControllerInstance.SetCommunicatingPlayer(userId, userName);

        //foreach (var userId in users[1])
        //{
        //    if (!_players.TryGetValue(userId, out GameObject player))
        //    {
        //        SpawnOtherPlayer(userId);
        //    }
        //}
    }

    //対象のユーザーを取得するメソッド
    private Transform GetUser(string userId)
    {
        Transform retUser = null;
        foreach (Transform user in otherPlayersParent.GetComponent<MultiPlayerParent>().players)
        {
            if (user.GetComponent<MultiPlayer>().myData == null) continue;
            if (user.GetComponent<MultiPlayer>().myData.userId != userId) continue;
            retUser = user;
        }
        return retUser;
    }

    public void OnMove(string userId, MyVector3 position, MyQuaternion quaternion)
    {
        if (selfId == userId) return;
        Transform user = GetUser(userId);
        user.SetPositionAndRotation(position.ToUnityVector3(), quaternion.ToUnityQuaternion());
    }

    public void OnSeedData(string userId,
                           List<int> numbers,
                           MyVector3[] position,
                           MyQuaternion[] quaternion)
    {
        if (selfId == userId) return;
        Transform user = GetUser(userId);
        user.GetComponent<MultiPlayer>().seedManagerScript.ReceptionSeedDataInput(
            numbers,
            position,
            quaternion);
    }

    public void OnSeedDestroy(string userId, int number)
    {
        if (selfId == userId) return;
        Transform user = GetUser(userId);
        user.GetComponent<MultiPlayer>().seedManagerScript.ReceptionSeedDestroy(number);
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
        OnGameStartCallBack();
    }

    public void OnPlayStart()
    {
        OnPlayStartCallBack();
    }

    public void OnMatchingTimerCount(float time)
    {
        if (myControllerInstance.isHost) return;
        if (OnTimerDisplayCallBack == null) return;
        OnTimerDisplayCallBack(time);
    }

    public void OnSwitchingHost(MyPlayerData inUser, bool inHost)
    {
        OtherPlayer user = inUser.ToOtherPlayer();
        if (selfId == user.userId) OnCheckHostCallBack(inHost);
    }

    public void OnHamsterMove(int count, MyVector3[] position, MyQuaternion[] quaternion)
    {
        if (myControllerInstance.isHost) return;
        OnHamsterMoveCallBack(count, position, quaternion);
    }

    public void OnHamsterDestroy(int number)
    {
        if (myControllerInstance.isHost) return;
        OnHamsterDestroyCallBack(number);
    }
}