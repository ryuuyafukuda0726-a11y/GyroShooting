using UnityEngine;

//ひまわりの種用スクリプトクラス
public class SunflowerSeed : MonoBehaviour
{
    //弾速用変数
    [SerializeField]
    private float speed = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //前進用メソッド
    private void MoveForward()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        MoveForward();
    }
}
