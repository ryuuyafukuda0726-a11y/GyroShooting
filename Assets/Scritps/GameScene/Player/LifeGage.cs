using UnityEngine;
using UnityEngine.UI;

//ライフゲージ用スクリプトクラス
public class LifeGage : MonoBehaviour
{
    //ソースイメージ用変数
    private Sprite[] mySprite;
    //イメージコンポーネント用変数
    private Image myImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myImage = GetComponent<Image>();
        mySprite = Resources.LoadAll<Sprite>("Image/GameScene/LifeGage");
    }

    //初期設定用メソッド
    public void Init()
    {
        myImage.sprite = mySprite[10];
    }

    //表示用メソッド
    public void Display(int index)
    {
        myImage.sprite = mySprite[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
