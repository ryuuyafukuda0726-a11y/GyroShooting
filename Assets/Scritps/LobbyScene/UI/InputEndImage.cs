using System;
using System.Collections;
using UnityEngine;
using TMPro;

//名前の入力完了ButtonのPanel用スクリプトクラス
public class InputEndImage : MonoBehaviour
{
    //RectTransform用変数
    private RectTransform rt;
    //イージング用変数
    private EasingControl easing = global::EasingControl.SetEasing;
    private Vector3 aScale, bScale;
    private float percent = 0.0f;
    private const float minPercent = 0.0f;
    private const float maxPercent = 1.0f;
    //入力完了用変数
    [NonSerialized]
    public bool isEnd = false;
    private bool isReturn = false;
    //Click時のCallBack用変数
    public Action doClickSECallBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    //パネルの初期化用メソッド
    private void PanelInit()
    {
        rt.transform.localScale = Vector3.up;
        transform.gameObject.SetActive(false);
    }

    //Flagの初期化用メソッド
    private void FlagInit()
    {
        isEnd = false;
        isReturn = false;
    }

    //初期設定用メソッド
    public void Init()
    {
        easing = global::EasingControl.SetEasing;
        PanelInit();
        FlagInit();
    }

    //イージング設定用メソッド
    private void SetEasing(string inMove)
    {
        //引数でイージングの内容をSwitch
        switch (inMove)
        {
            case "Open":
                aScale = Vector3.up;
                bScale = Vector3.one;
                break;
            case "Close":
                aScale = Vector3.one;
                bScale = Vector3.up;
                break;
            default:
                break;
        }
        percent = minPercent;
    }

    //イージング用メソッド
    private bool Easing()
    {
        float speed = 2.0f;
        percent += Time.deltaTime * speed;
        float calculationPercent = Mathf.Pow(percent, 2.0f);
        rt.transform.localScale = Vector3.Lerp(aScale, bScale, calculationPercent);
        return percent >= maxPercent ? true : false;
    }

    //イージング管理用メソッド
    public bool EasingControl()
    {
        bool retBool = false;
        //イージング進行状態でSwitch
        switch (easing)
        {
            case global::EasingControl.SetEasing:
                SetEasing("Open");
                easing++;
                break;
            case global::EasingControl.Easing:
                //イージングの実行状態を確認
                if (Easing()) easing++;
                break;
            case global::EasingControl.EasingEnd:
                if (isReturn) InputReturn();
                retBool = true;
                break;
            default:
                break;
        }
        return retBool;
    }

    //入力のやり直し用メソッド
    private void InputReturn()
    {
        InputNameImage inputNameImageScript = transform.parent.GetComponent<InputNameImage>();
        inputNameImageScript.nameTMP.GetComponent<TextMeshProUGUI>().text = "";
        inputNameImageScript.isInputEnd = false;
        transform.gameObject.SetActive(false);
        easing = global::EasingControl.SetEasing;
        isReturn = false;
    }

    //OkButtonの入力用メソッド
    public void OnOkClick()
    {
        //doClickSECallBack();
        SetEasing("Close");
        easing = global::EasingControl.Easing;
        isEnd = true;
    }

    //NoButtonの入力用メソッド
    public void OnNoClick()
    {
        //doClickSECallBack();
        SetEasing("Close");
        easing = global::EasingControl.Easing;
        isReturn = true;
    }
}
