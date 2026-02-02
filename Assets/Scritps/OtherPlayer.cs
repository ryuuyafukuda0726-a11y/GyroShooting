using UnityEngine;

//その他のプレイヤー用スクリプトクラス
public class OtherPlayer
{
    //その他のプレイヤーの情報用変数
    public string userId = "";
    public string userName = "";

    //コンストラクター
    public OtherPlayer(string inId, string inName)
    {
        userId = inId;
        userName = inName;
    }
}
