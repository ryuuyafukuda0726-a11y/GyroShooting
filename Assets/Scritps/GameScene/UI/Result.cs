using UnityEngine;

//リザルトUI用スクリプトクラス
public class Result : MonoBehaviour
{
    //レクトトランスフォームコンポーネント用変数
    private RectTransform rt;
    //ソースイメージ用変数
    [SerializeField]
    private Sprite gameClear;
    [SerializeField]
    private Sprite gameOver;
    //イージング用変数
    private EasingControl easing = EasingControl.SetEasing;
    private Vector3 aVec, bVec;
    private Quaternion aRot, bRot;
    private float percent = 0.0f;
    private const float maxPercent = 1.0f;
    private const float minPercent = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    //初期設定用メソッド
    public void Init()
    {
       rt.transform.gameObject.SetActive(false);
    }

    //イージング設定用メソッド
    private void SetEasing(bool inClear)
    {
        rt.transform.gameObject.SetActive(true);
        if (inClear)
        {
            aVec = Vector3.zero;
            bVec = Vector3.one;
        }
        else
        {
            aVec = rt.transform.localPosition + Vector3.up * 600.0f;
            bVec = rt.transform.localPosition;
            aRot = Quaternion.identity;
            bRot = new Quaternion(0.0f, 0.0f, 0.25f, 0.0f);
        }
        percent = minPercent;
    }

    //イージング用メソッド
    private bool Easing(bool inClear)
    {
        percent += Time.deltaTime;
        if (inClear)
        {
            rt.transform.localScale = Vector3.Lerp(aVec, bVec, percent);
            rt.transform.Rotate(Vector3.forward * 3600.0f * Time.deltaTime);
            if(percent > maxPercent)rt.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }
        else
        {
            rt.transform.localPosition = Vector3.Lerp(aVec, bVec, percent);
            rt.transform.localRotation = Quaternion.Lerp(aRot, bRot, percent);
        }
        return percent >= maxPercent;
    }

    //イージング管理用メソッド
    public bool ResultEasingControl(bool inClear)
    {
        //イージングの進行状態でスイッチ
        switch (easing)
        {
            case EasingControl.SetEasing:
                SetEasing(inClear);
                easing++;
                break;
            case EasingControl.Easing:
                if(Easing(inClear)) easing++;
                break;
            case EasingControl.EasingEnd:
                easing = EasingControl.SetEasing;
                return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
