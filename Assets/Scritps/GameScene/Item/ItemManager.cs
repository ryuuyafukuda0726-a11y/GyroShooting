using System.Collections.Generic;
using UnityEngine;

//アイテム管理用スクリプトクラス
public class ItemManager
{
    //アイテム管理用変数
    private int maxItemCount = 0;
    private GameObject itemObject;
    private GameObject[] itemObjects;
    private List<GameObject> itemList;
    private GameObject itemParent;

    //変数の生成用メソッド
    private void CreateArray()
    {
        itemObjects = new GameObject[maxItemCount * 2];
        itemParent = new GameObject("ItemParent");
    }

    //コンストラクター
    public ItemManager(int inMaxCount, GameObject inItem)
    {
        maxItemCount = inMaxCount;
        itemObject = inItem;
    }
}
