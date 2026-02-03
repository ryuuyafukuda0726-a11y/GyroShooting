using System;
using UnityEngine;

//餌箱トラップ用スクリプトクラス
public class FeedingBox : MonoBehaviour
{
    //ダメージ用変数
    [SerializeField]
    private int damageValue = 0;
    //トラップの耐久用変数
    [SerializeField]
    private int maxDurability = 0;
    private int durability = 0;
    //箱のモデル用変数
    [SerializeField]
    private GameObject[] boxObjects = new GameObject[4];
    //接触判定用変数
    private bool isFlag = true;
    //コールバック用変数
    public Action<Transform> destroyCallBack;

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
        durability = maxDurability;
        boxObjects[0].SetActive(true);
    }

    //設置入力用メソッド
    public void InputInstallation()
    {
        isFlag = true;
    }

    //ダメージ量の取得用メソッド
    public int GetDamageValue()
    {
        return damageValue;
    }

    //耐久値減少用メソッド
    public void durabilityDown()
    {
        durability--;
        if (durability > 0) return;
        destroyCallBack(transform);
    }

    //接触判定用メソッド
    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag != "Wall" && other.tag != "SunFlower")
        {
            isFlag = true;
        }
        else isFlag = false;
        Debug.Log(isFlag);
    }

    //接触確認用メソッド
    public bool GetFlag()
    {
        Debug.Log(isFlag);
        return isFlag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
