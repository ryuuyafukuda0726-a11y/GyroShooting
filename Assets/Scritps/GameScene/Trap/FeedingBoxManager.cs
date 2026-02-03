using UnityEngine;
using System.Collections.Generic;
using System;

//餌箱トラップを管理するマネージャースクリプトクラス
public class FeedingBoxManager
{
    //餌箱用変数
    private GameObject feedingBoxPrefab;
    private int maxTrap = 0;
    private int trapCount = 0;
    private int trapNumber = 1;
    private GameObject[] feedingBoxObjects;
    private GameObject feedingBoxParent;
    private List<GameObject> feedingBoxs = new List<GameObject>();
    //コールバック用変数
    public Action<List<GameObject>> setTrapListCallBack;
    public Action setTrapCallBack;

    //餌箱の生成用メソッド
    private void CreateFeedingBox()
    {
        feedingBoxParent = new GameObject("FeedingBoxParent");
        feedingBoxObjects = new GameObject[trapCount];
        for (int i = 0; i < trapCount; i++)
        {
            feedingBoxObjects[i] = GameObject.Instantiate(feedingBoxPrefab, feedingBoxParent.transform);
            feedingBoxObjects[i].GetComponent<FeedingBox>().Init();
            feedingBoxObjects[i].GetComponent<FeedingBox>().destroyCallBack = destroyTrap;
            feedingBoxObjects[i].SetActive(false);
        }
    }

    //初期設定用メソッド
    public void Init()
    {
        CreateFeedingBox();
    }

    //リストの登録用メソッド
    private void SetList(Vector3 inPos)
    {
        feedingBoxObjects[trapNumber].SetActive(true);
        feedingBoxObjects[trapNumber].transform.position = inPos;
        feedingBoxObjects[trapNumber].GetComponent<FeedingBox>().Installation();
        feedingBoxs.Add(feedingBoxObjects[trapNumber]);
    }

    //設置状態の確認用メソッド
    private bool CheckInstallation()
    {
        for(int i = 0; i < feedingBoxs.Count; i++)
        {
            if (feedingBoxObjects[trapNumber] == feedingBoxs[i]) return false;                
        }
        return true;
    }

    //生成用メソッド
    private void CreateTrap(Vector3 inPos)
    {
        bool isFlag = false;
        while (!isFlag)
        {
            isFlag = CheckInstallation();
            if(isFlag) SetList(inPos);
            trapNumber++;
            if (trapNumber >= trapCount) trapNumber = 1;
        }        
    }

    //設置用メソッド
    public void Installation(Vector3 inPos)
    {
        if (feedingBoxs.Count >= maxTrap) return;
        if (feedingBoxs.Count == 0) SetList(inPos);
        else CreateTrap(inPos);
        SetTrapList();
    }

    //設置入力用メソッド
    public void InputInstallation()
    {
        feedingBoxObjects[0].SetActive(true);
        feedingBoxObjects[0].transform.GetChild(0).gameObject.SetActive(true);
        feedingBoxObjects[0].GetComponent<FeedingBox>().InputInstallation();
    }

    //設置場所確認用メソッド
    public void CheckInstallationSpace(Vector3 inPos)
    {
        feedingBoxObjects[0].transform.position = inPos;
    }

    //確認終了時用メソッド
    public void CheckInstallationSpaceEnd(Vector3 inPos, bool inCost)
    {
        bool isFlag = feedingBoxObjects[0].GetComponent<FeedingBox>().GetFlag();
        Debug.Log("設置可能 : " + isFlag + ", コスト : " + inCost );
        if (isFlag && inCost)
        {
            Installation(inPos);
            setTrapCallBack();
        }
        feedingBoxObjects[0].SetActive(false);
        feedingBoxObjects[0].transform.position = Vector3.zero;
    }

    //トラップのリスト登録用メソッド
    private void SetTrapList()
    {
        setTrapListCallBack(feedingBoxs);
    }

    //トラップ破壊コールバック用メソッド
    private void destroyTrap(Transform inTrap)
    {
        for(int i = 0; i < feedingBoxs.Count; i++)
        {
            if (feedingBoxs[i] != inTrap.gameObject) continue;
            feedingBoxs.RemoveAt(i);
            inTrap.gameObject.SetActive(false);
            inTrap.position = Vector3.zero;
        }
        SetTrapList();
    }

    //コンストラクター
    public FeedingBoxManager(GameObject inFeedingBoxPrefab, int inMaxTrap, int inTrapCount)
    {
        feedingBoxPrefab = inFeedingBoxPrefab;
        maxTrap = inMaxTrap;
        trapCount = inTrapCount;
    }
}
