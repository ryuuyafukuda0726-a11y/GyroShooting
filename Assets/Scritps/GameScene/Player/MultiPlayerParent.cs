using UnityEngine;
using UnityEngine.Playables;

//マルチプレイヤーを管理する親オブジェクト用スクリプトクラス
public class MultiPlayerParent : MonoBehaviour
{
    //プレイヤー用変数
    [SerializeField]
    public Transform[] players = new Transform[3];
    //オーディオソース用変数
    private GameAudioSource audioSource;
    //通信用変数
    private MagicOnionController myControllerInstance = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myControllerInstance = MagicOnionController.GetInstance;
    }

    //対象のプレイヤー情報を登録
    private void SetPlayerData(int inNumber)
    {
        players[inNumber].gameObject.SetActive(true);
        players[inNumber].GetComponent<MultiPlayer>().myData
            = myControllerInstance.otherPlayers[inNumber];
        players[inNumber].GetComponent<MultiPlayer>().Init(audioSource); ;
    }

    //すべての子オブジェクトのアクティブ状態を登録
    private void AllChildSetActive()
    {
        int size = players.Length;
        if (myControllerInstance == null || !myControllerInstance.isMulti)
        {
            for (int i = 0; i < size; i++)
            {
                players[i].gameObject.SetActive(false);
            }
        }
        else if (myControllerInstance.isMulti)
        {
            for (int i = 0; i < size; i++) 
            {
                if (myControllerInstance.otherPlayers.Count > i)SetPlayerData(i);
                else players[i].gameObject.SetActive(false);
            }
        }
    }

    //初期設定用メソッド
    public void Init(GameAudioSource inAs)
    {
        audioSource = inAs;
        AllChildSetActive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
