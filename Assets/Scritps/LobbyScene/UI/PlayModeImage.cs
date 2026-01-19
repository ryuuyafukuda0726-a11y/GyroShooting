using UnityEngine;

//プレイモード選択画面用スクリプトクラス
public class PlayModeImage : MonoBehaviour
{
    //レクトトランスフォーム用変数
    private RectTransform rt;
    //イージング用変数
    private EasingControl easing;
    private Vector3 aVec, bVec;
    private float percent = 0.0f;
    private const float maxPercent = 1.0f;
    private const float minPercent = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    //イージング設定用メソッド
    private void SetEasing(string inMove)
    {
        //引数でイージングの内容をSwitch
        switch (inMove)
        {
            case "Open":
                aVec = Vector3.up;
                bVec = Vector3.one;
                break;
            case "Close":
                aVec = Vector3.one;
                bVec = Vector3.up;
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
        rt.transform.localScale = Vector3.Lerp(aVec, bVec, percent);
        return percent >= maxPercent ? true : false;
    }

    //イージング管理用メソッド
    public bool EasingControl(string inMove)
    {
        //イージング進行状態でSwitch
        switch (easing)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
