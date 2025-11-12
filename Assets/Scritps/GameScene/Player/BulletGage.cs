using UnityEngine;
using UnityEngine.UI;

//残弾ゲージ用スクリプトクラス
public class BulletGage : MonoBehaviour
{
    //残弾用変数
    private const int maxSeed = 100;
    //ソースイメージ用変数
    private Sprite[] mySprite;
    //イメージコンポーネント用変数
    private Image myImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myImage = GetComponent<Image>();
        mySprite = Resources.LoadAll<Sprite>("Image/GameScene/BulletGage");
    }

    //初期設定用メソッド
    public void Init()
    {
        myImage.sprite = mySprite[0];
    }

    //表示用メソッド
    public void Display(int inSeed)
    {
        int index = maxSeed - inSeed;
        myImage.sprite = mySprite[index / 10];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
