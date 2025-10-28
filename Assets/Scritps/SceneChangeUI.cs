using UnityEngine;
using UnityEngine.UI;

//タイトルUI用スクリプトクラス
public class SceneChangeUI : MonoBehaviour
{
    //マテリアル用変数
    private Material myMaterial;
    private int holeRadiusID;
    //イージング用変数
    private EasingControl easing = global::EasingControl.SetEasing;
    private float aRadius, bRadius;
    private float radius = 0.0f;
    private const float maxRadius = 0.5f;
    private const float minRadius = 0.0f;
    private float percent = 0.0f;
    private const float maxPercent = 1.0f;
    private const float minPercent = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myMaterial = GetComponent<Image>().material;
        holeRadiusID = Shader.PropertyToID("_HoleRadius");
    }

    //初期設定用メソッド
    public void Init()
    {
        myMaterial.SetFloat(holeRadiusID, minRadius);
    }

    //イージング設定用メソッド
    private void SetEasing(string inMove)
    {
        //引数で設定をswitch
        switch (inMove)
        {
            case "Open":
                aRadius = minRadius;
                bRadius = maxRadius;
                break;
            case "Close":
                aRadius = maxRadius;
                bRadius = minRadius;
                break;
            default:
                break;
        }
        percent = minPercent;
    }

    //イージング用メソッド
    private bool Easing()
    {
        percent += Time.deltaTime;

        float y = Mathf.Pow(percent, 3.0f) + 2.0f * Mathf.Pow(percent, 2.0f) - percent / 2.0f;

        radius = Mathf.Lerp(aRadius, bRadius, percent * percent);
        myMaterial.SetFloat(holeRadiusID, radius);
        return percent >= maxPercent ? true : false;
    }

    //イージング管理用変数
    public bool EasingControl(string inMove)
    {
        //イージングの進行状態でswitch
        switch(easing)
        {
            case global::EasingControl.SetEasing:
                SetEasing(inMove);
                easing++;
                break;
            case global::EasingControl.Easing:
                //イージングの実行状態を確認
                if (Easing()) easing++;
                break;
            case global::EasingControl.EasingEnd:
                easing = global::EasingControl.SetEasing;
                return true;
            default:
                break;
        }
        return false;
    }
}
