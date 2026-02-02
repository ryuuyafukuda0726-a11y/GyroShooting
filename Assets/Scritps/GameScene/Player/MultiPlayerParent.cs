using UnityEngine;
using UnityEngine.Playables;

//マルチプレイヤーを管理する親オブジェクト用スクリプトクラス
public class MultiPlayerParent : MonoBehaviour
{
    //通信用変数
    private MagicOnionController myControllerInstance = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myControllerInstance = MagicOnionController.GetInstance;
    }

    //すべての子オブジェクトのアクティブ状態を登録
    private void AllChildSetActive()
    {
        int size = transform.childCount;
        if (myControllerInstance == null)
        {
            for (int i = 0; i < size; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        else if (myControllerInstance.isMulti)
        {
            for (int i = 0; i < size; i++) 
            {
                if (myControllerInstance.otherPlayers.Count > i) transform.GetChild(i).gameObject.SetActive(true);
                else transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    //初期設定用メソッド
    public void Init()
    {
        AllChildSetActive();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
