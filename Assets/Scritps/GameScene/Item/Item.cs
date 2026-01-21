using System;
using UnityEngine;

//アイテムの種類
enum ItemType
{
    RapidFire,
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
    //色用変数
    [SerializeField]
    private Color[] itemColor 
        = new Color[Enum.GetValues(typeof(ItemType)).Length];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainModule = ps.main;
        myRend = itemObject.GetComponent<Renderer>();
    }

    //出現時用メソッド
    public void Spawn(int inItemType)
    {
        mainModule.startColor = itemColor[inItemType];
        myRend.material.color = itemColor[inItemType];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
