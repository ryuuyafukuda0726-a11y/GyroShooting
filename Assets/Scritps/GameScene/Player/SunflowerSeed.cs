using System;
using UnityEngine;

//ひまわりの種用スクリプトクラス
public class SunflowerSeed : MonoBehaviour
{
    [NonSerialized]
    public int number = 0;
    //ダメージ用変数
    [NonSerialized]
    public int damage = 0;
    //弾速用変数
    [SerializeField]
    private float bulletSpeed = 0.0f;
    [SerializeField]
    private float rotSpeed = 0.0f;
    //重力加速度用変数
    private const float g = 9.8f;
    //移動用変数
    private Vector3 value;
    //存在する時間用変数
    [SerializeField]
    private float lifeTime = 0.0f;
    private float myTime = 0.0f;
    //発射確認用変数
    private bool isShot = false;
    //ひまわりのモデル用変数
    [SerializeField]
    private GameObject sunflowerSeedObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //発射用メソッド
    public void Shot(int inDamageValue, Vector3 inValue, int inNumber)
    {
        number = inNumber;
        myTime = 0.0f;
        value = inValue * bulletSpeed;
        //Debug.Log(value);
        damage = inDamageValue;
        //float dot = Vector3.Dot((transform.position - transform.forward).normalized, Vector3.up);
        //float y = bulletSpeed * dot;
        //dot = Vector3.Dot(transform.forward, Vector3.forward);
        //float z = (bulletSpeed - y) * dot;
        //float x = (bulletSpeed - y) * (1.0f - dot);
        //value = new Vector3(x, y, z);
        isShot = true;
    }

    //移動用メソッド
    private void Move()
    {
        transform.Translate(value * Time.deltaTime);
        //Debug.Log(value * Time.deltaTime);
        sunflowerSeedObject.transform.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);        
    }

    //重力加速度用メソッド
    private void GravitationalAcceleration()
    {
        value += Vector3.down * g * Time.deltaTime;

        //bulletSpeed += -g * Time.deltaTime;
        //transform.Translate(new Vector3(0.0f, bulletSpeed, 0.0f) * Time.deltaTime);
    }

    //存在している時間の確認用メソッド
    private void CheckLifeTime()
    {
        myTime += Time.deltaTime;
        if (myTime < lifeTime) return;
        DisappearanceAndHitDetection();
    }

    //消滅及び当たり判定時用メソッド
    public void DisappearanceAndHitDetection()
    {
        isShot = false;
        transform.position = transform.parent.position;
        transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShot) return;
        Move();
        //Debug.Log(transform.position);
        GravitationalAcceleration();
        //Debug.Log(transform.position);
        CheckLifeTime();
        //Debug.Log(transform.position);
    }

    //ダメージ量の取得用メソッド
    public int GetDamageValue()
    {
        return damage;
    }
}
