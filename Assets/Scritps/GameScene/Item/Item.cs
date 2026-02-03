using System;
using UnityEngine;

//アイテムの種類
enum ItemType
{
    SpeedUp,
    BulletSupply,
    Shooting,
    Recovery
}

//Item用スクリプトクラス
public class Item : MonoBehaviour
{
    //アイテム用変数
    [SerializeField]
    private ParticleSystem ps;
    private ParticleSystem.MainModule mainModule;
    private Renderer myRend;
    [SerializeField]
    private GameObject itemObject;
    private int itemType = 0;
    [SerializeField]
    private float deleteTime = 0.0f;
    //色用変数
    [SerializeField]
    private Color[] itemColor 
        = new Color[Enum.GetValues(typeof(ItemType)).Length];
    //時間管理用変数
    private float myTime = 0.0f;
    //コールバック用メソッド
    public Action<Transform> deleteCallBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //初期設定用メソッド
    public void Init()
    {
        mainModule = ps.main;
        myRend = itemObject.GetComponent<Renderer>();
        myTime = 0.0f;
    }

    //出現時用メソッド
    public void Spawn(int inItemType)
    {
        mainModule.startColor = itemColor[inItemType];
        myRend.material.color = itemColor[inItemType];
        itemType = inItemType;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Player") return;
        collision.transform.GetComponent<Player>().SetItem(itemType);
        deleteCallBack(transform);
    }

    //消滅時間管理用メソッド
    private bool CheckDeleteTime()
    {
        myTime += Time.deltaTime;
        if(myTime > deleteTime)
        {
            myTime = 0.0f;
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckDeleteTime()) return;
        deleteCallBack(transform);
    }
}
