using System.Collections.Generic;
using UnityEngine;

//アイテム管理用スクリプトクラス
public class ItemManager
{
    //アイテム管理用変数
    private int maxItemCount = 0;
    private GameObject itemPrefab;
    private GameObject[] itemObjects;
    private List<GameObject> itemList;
    private GameObject itemParent;

    //アイテムの生成用メソッド
    private void CreateItem()
    {
        int size = itemObjects.Length;
        for(int i = 0; i < size; i++)
        {
            itemObjects[i] = GameObject.Instantiate(itemPrefab, itemParent.transform);
            itemObjects[i].SetActive(false);
        }
    }

    //変数の生成用メソッド
    private void CreateArray()
    {
        itemObjects = new GameObject[maxItemCount * 2];
        itemParent = new GameObject("ItemParent");
        itemList = null;
    }

    //コンストラクター
    public ItemManager(int inMaxCount, GameObject inItem)
    {
        maxItemCount = inMaxCount;
        itemPrefab = inItem;
        CreateArray();
        CreateItem();
    }
}
