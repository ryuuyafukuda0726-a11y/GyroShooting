using System;
using UnityEngine;
using UnityEngine.InputSystem;

//ひまわり用スクリプトクラス
public class SunFlower : MonoBehaviour
{
    //残弾回復用変数
    [SerializeField]
    private float chargeBulletInterval = 0.0f;
    [SerializeField]
    private float chargeDistance = 0.0f;
    private float myTime = 0.0f;
    //耐久値用変数
    private const int maxHp = 100;
    private int hp = maxHp;
    //プレイヤー用変数
    [SerializeField]
    private Transform player;
    //コールバック用変数
    public Action<float> sunFlowerGageDisplayCallBack;
    public Action chargeBulletCallBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //距離の判定用メソッド
    private bool CheckDistance()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance <= chargeDistance ? true : false;
    }

    //プレイヤーへの残弾補充用メソッド
    private void PlayerChargeBullet()
    {
        myTime += Time.deltaTime;
        if(myTime > chargeBulletInterval)
        {
            myTime = 0.0f;
            if (!CheckDistance()) return;
            chargeBulletCallBack();
        }
    }

    //プレイ用メソッド
    public void Play()
    {
        PlayerChargeBullet();
        sunFlowerGageDisplayCallBack(hp);
    }

    //ダメージ用メソッド
    public void Damage()
    {
        if (hp <= 0) return;
        hp--;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
