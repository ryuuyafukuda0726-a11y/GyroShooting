using UnityEngine;

//餌箱トラップ用スクリプトクラス
public class FeedingBox : MonoBehaviour
{
    //箱のモデル用変数
    [SerializeField]
    private GameObject[] boxObjects = new GameObject[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //初期設定用メソッド
    public void Init()
    {
        for(int i = 0; i < boxObjects.Length; i++)
        {
            boxObjects[i].SetActive(false);
        }
    }

    //設置用メソッド
    public void Installation()
    {
        Init();
        boxObjects[0].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
