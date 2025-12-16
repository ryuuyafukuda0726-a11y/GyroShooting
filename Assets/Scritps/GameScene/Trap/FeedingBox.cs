using UnityEngine;

//餌箱トラップ用スクリプトクラス
public class FeedingBox : MonoBehaviour
{
    //箱のモデル用変数
    [SerializeField]
    private GameObject[] boxObjects = new GameObject[4];
    //接触判定用変数
    private bool isFlag = false;

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

    //設置入力用メソッド
    public void InputInstallation()
    {
        isFlag = false;
    }

    //接触判定用メソッド
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Untagged")
        {
            isFlag = true;
        }
        else isFlag = false;
        Debug.Log(isFlag);
    }

    //接触確認用メソッド
    public bool GetFlag()
    {
        return isFlag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
