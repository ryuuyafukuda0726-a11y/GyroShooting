using UnityEngine;
using UnityEngine.UI;

//画面タッチの指示UI用スクリプトクラス
public class TouchScreenImage : MonoBehaviour
{
    //イメージコンポーネント用変数
    private Image myImage;
    //表示用変数
    private float value = 0.0f;
    private bool flag = false;
    const float minAlpha = 0.05f;
    const float maxAlpha = 0.95f;
    const float displaySpeed = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myImage = GetComponent<Image>();
    }

    //初期設定用メソッド
    public void Init()
    {
        myImage.material.color = Color.clear;
    }

    //UIの表示用メソッド
    public void DisplayUI()
    {
        value += flag ? Time.deltaTime : -Time.deltaTime;
        float alpha = Mathf.Sqrt(Mathf.Pow(1.0f, 2.0f) - Mathf.Pow(value * displaySpeed, 2.0f));
        if (alpha < minAlpha) flag = true;
        else if (alpha > maxAlpha) flag = false;
        myImage.material.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Pow(alpha, 2.0f));
    }
}
