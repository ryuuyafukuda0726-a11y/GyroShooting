using System;
using UnityEngine;
using UnityEngine.InputSystem;

//ひまわり用スクリプトクラス
public class SunFlower : MonoBehaviour
{
    //耐久値用変数
    private const int maxHp = 100;
    private int hp = maxHp;
    //コールバック用変数
    public Action<float> sunFlowerGageDisplayCallBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //ダメージ用メソッド
    public void Damage()
    {
        if (hp <= 0) return;
        hp--;
    }

    //プレイ用メソッド
    public void Play()
    {
        sunFlowerGageDisplayCallBack(hp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
