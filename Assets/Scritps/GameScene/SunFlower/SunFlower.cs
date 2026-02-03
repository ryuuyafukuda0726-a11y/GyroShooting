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
    [SerializeField]
    private int  maxHp = 0;
    private int hp = 0;
    //プレイヤー用変数
    [SerializeField]
    private Transform player;
    //イージング用変数
    [SerializeField]
    private float breakSpeed = 0.0f;
    private EasingSequence easingSequence = EasingSequence.SetEasing;
    private float percent = 0.0f;
    private const float maxPercent = 1.0f;
    private const float minPercent = 0.0f;
    private Quaternion startRot, endRot;
    //コールバック用変数
    public Action<float> sunFlowerGageDisplayCallBack;
    public Action chargeBulletCallBack;
    public Action<bool> destroyCallBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hp = maxHp;
        easingSequence = EasingSequence.SetEasing;
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

    //イージング設定用メソッド
    private void SetEasing()
    {
        startRot = transform.rotation;
        endRot = new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);
        percent = minPercent;
    }

    //イージング用メソッド
    private bool Easing()
    {
        percent += Time.deltaTime * breakSpeed;
        transform.rotation = Quaternion.Lerp(startRot, endRot, percent);
        transform.Rotate(Vector3.up * breakSpeed * Time.deltaTime);
        return percent >= maxPercent ? true : false;
    }

    //イージング管理用メソッド
    private void EasingControl()
    {
        switch (easingSequence)
        {
            case EasingSequence.SetEasing:
                SetEasing();
                easingSequence++;
                break;
            case EasingSequence.Easing:
                if (Easing()) easingSequence++;
                break;
            case EasingSequence.EasingEnd:
                destroyCallBack(false);
                easingSequence = EasingSequence.SetEasing;
                break;
            default:
                break;
        }
    }

    //破壊時用メソッド
    private void Destroy()
    {
        if (hp > 0) return;
        EasingControl();
    }

    //プレイ用メソッド
    public void Play()
    {
        Destroy();
        sunFlowerGageDisplayCallBack(hp);
        if (hp <= 0) return;
        PlayerChargeBullet();
    }

    //ダメージ用メソッド
    public void Damage(int inDamage)
    {
        if (hp <= 0) return;
        hp -= inDamage;
    }

    //最大HPの取得用メソッド
    public float GetMaxHP()
    {
        return maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
