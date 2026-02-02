using UnityEngine;
using MagicOnionStudy.Shared; 
public static class PlayerDataExtensions
{
    public static MyPlayerData ToMyPlayerData(this OtherPlayer v)
    {
        return new MyPlayerData(v.userId, v.userName);
    }

    public static OtherPlayer ToOtherPlayer(this MyPlayerData v)
    {
        return new OtherPlayer(v.userId, v.userName);
    }
}