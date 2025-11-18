using System;
using UnityEngine;

//ハムスターのモデル用スクリプトクラス
public class Hamster_Model : MonoBehaviour
{
    //コールバック用メソッド
    public Action AttackCallBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //攻撃時のアニメーションイベント用メソッド
    public void AttackAnimationEvent()
    {
        AttackCallBack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
