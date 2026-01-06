using UnityEngine;
using UnityEngine.Rendering;

//ひまわりの耐久ゲージ用スクリプトクラス
public class SunFlowerGage : MonoBehaviour
{
    //ゲージ用変数
    [SerializeField]
    private RectTransform gage;
    private float maxHp = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //初期設定用メソッド
    public void Init(float inHp)
    {
        maxHp = inHp;
    }

    //表示用メソッド
    public void Display(float inHp)
    {
        Vector2 size = new Vector2(gage.sizeDelta.x, 300.0f * ((maxHp - inHp) / maxHp));
        gage.sizeDelta = size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
