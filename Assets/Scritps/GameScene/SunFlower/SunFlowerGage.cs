using UnityEngine;

//ひまわりの耐久ゲージ用スクリプトクラス
public class SunFlowerGage : MonoBehaviour
{
    //ゲージ用変数
    [SerializeField]
    private RectTransform gage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //表示用メソッド
    public void Display(float inHp)
    {
        Vector2 size = new Vector2(gage.sizeDelta.x, 300.0f * ((100.0f - inHp) / 100.0f));
        Debug.Log(inHp);
        gage.sizeDelta = size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
