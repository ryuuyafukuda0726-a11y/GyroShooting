using UnityEngine;

//ひまわりの種用スクリプトクラス
public class SunflowerSeed : MonoBehaviour
{
    //弾速用変数
    [SerializeField]
    private float bulletSpeed = 0.0f;
    [SerializeField]
    private float rotSpeed = 0.0f;
    //存在する時間用変数
    [SerializeField]
    private float lifeTime = 0.0f;
    private float myTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //前進用メソッド
    private void MoveForward()
    {
        transform.Translate(transform.forward * bulletSpeed * Time.deltaTime);
        transform.Rotate(transform.forward * rotSpeed * Time.deltaTime);
    }

    //存在している時間の確認用メソッド
    private void CheckLifeTime()
    {
        myTime += Time.deltaTime;
        if (myTime < lifeTime) return;
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
        CheckLifeTime();
    }
}
