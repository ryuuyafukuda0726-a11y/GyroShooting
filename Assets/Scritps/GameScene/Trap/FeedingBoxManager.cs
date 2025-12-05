using UnityEngine;
using System.Collections.Generic;

//餌箱トラップを管理するマネージャースクリプトクラス
public class FeedingBoxManager
{
    //餌箱用変数
    private GameObject feedingBoxPrefab;
    private int maxTrap = 0;
    private int trapCount = 0;
    private int trapNumber = 0;
    private GameObject[] feedingBoxObjects;
    private GameObject feedingBoxParent;
    private List<GameObject> feedingBoxs = new List<GameObject>();

    //餌箱の生成用メソッド
    private void CreateFeedingBox()
    {
        feedingBoxParent = new GameObject("FeedingBoxParent");
        feedingBoxObjects = new GameObject[trapCount];
        for (int i = 0; i < trapCount; i++)
        {
            feedingBoxObjects[i] = GameObject.Instantiate(feedingBoxPrefab, feedingBoxParent.transform);
            feedingBoxObjects[i].GetComponent<FeedingBox>().Init();
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

    //設置用メソッド
    private void Installation(Vector3 inPos)
    {
        bool isFlag = false;
        while (!isFlag)
        {
            isFlag = CheckInstallation();
            if(isFlag) SetList(inPos);
            trapNumber++;
            if (trapNumber >= trapCount) trapNumber = 0;
        }        
    }

    //設置入力用メソッド
    public void InputInstallation(Vector3 inPos)
    {
        if (feedingBoxs.Count >= maxTrap) return;
        if (feedingBoxs.Count == 0) SetList(inPos);
        else Installation(inPos);
    }

    //コンストラクター
    public FeedingBoxManager(GameObject inFeedingBoxPrefab, int inMaxTrap, int inTrapCount)
    {
        feedingBoxPrefab = inFeedingBoxPrefab;
        maxTrap = inMaxTrap;
        trapCount = inTrapCount;
    }
}
