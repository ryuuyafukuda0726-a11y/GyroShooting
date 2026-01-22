using System;
using System.Collections.Generic;
using UnityEngine;

//アイテム管理用スクリプトクラス
public class ItemManager
{
    //アイテム管理用変数
    private int maxItemCount = 0;
    private GameObject itemPrefab;
    private GameObject[] itemObjects;
    private List<GameObject> itemList = new List<GameObject>();
    private GameObject itemParent;
    private int itemNumber = 0;

    //リスト登録用メソッド
    private void SetList(Vector3 inPos)
    {
        int itemType = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ItemType)).Length);
        itemObjects[itemNumber].SetActive(true);
        itemObjects[itemNumber].transform.position = inPos;
        itemObjects[itemNumber].GetComponent<Item>().Spawn(itemType);
        itemList.Add(itemObjects[itemNumber]);
    }

    //ドロップ状態の確認用メソッド
    private bool CheckDropItem()
    {
        int size = itemList.Count;
        for (int i = 0; i < size; i++)
        {
            if (itemObjects[itemNumber] == itemList[i]) return false;
        }
        return true;
    }

    //アイテムドロップ用メソッド
    private void ItemDrop(Vector3 inPos)
    {
        bool isFlag = false;
        while (!isFlag)
        {
            isFlag = CheckDropItem();
            if (isFlag) SetList(inPos);
            itemNumber++;
            if (itemNumber >= maxItemCount) itemNumber = 0;
        }
    }

    //アイテムドロップコールバック用メソッド
    public void ItemDropCallBack(Vector3 inPos)
    {
        Debug.Log(itemList);
        if (itemList.Count >= maxItemCount) return;
        if (/*itemList == null || */itemList.Count == 0) SetList(inPos);
        else ItemDrop(inPos);
    }

    //消滅コールバック用メソッド
    private void DeleteCallBack(Transform inItem)
    {
        int size = itemList.Count;
        for(int i = 0; i < size; i++)
        {
            if (itemList[i].transform != inItem) continue;
            itemList.RemoveAt(i);
        }
    }

    //アイテムの生成用メソッド
    private void CreateItem()
    {
        int size = itemObjects.Length;
        for(int i = 0; i < size; i++)
        {
            itemObjects[i] = GameObject.Instantiate(itemPrefab, itemParent.transform);
            itemObjects[i].GetComponent<Item>().deleteCallBack = DeleteCallBack;
            itemObjects[i].GetComponent<Item>().Init();
            itemObjects[i].SetActive(false);
        }
    }

    //変数の生成用メソッド
    private void CreateArray()
    {
        itemObjects = new GameObject[maxItemCount * 2];
        itemParent = new GameObject("ItemParent");
        //itemList = null;
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
