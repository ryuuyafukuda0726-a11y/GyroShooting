using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;
using UnityEngine.UIElements;
using System;
using TMPro;

//名前イメージ用スクリプトクラス
public class InputAddressImage : MonoBehaviour
{
    //アドレス用変数
    private int addressLength = 15;
    [NonSerialized]
    public string address = "";
    //イージング用変数
    private EasingControl easing = global::EasingControl.SetEasing;
    private Vector3 aScale, bScale;
    private float percent = 0.0f;
    private const float minPercent = 0.0f;
    private const float maxPercent = 1.0f;
    //RectTransform用変数
    private RectTransform rt;
    //TMP用変数
    public TextMeshProUGUI addressTMP;
    //入力完了Panel用変数
    [NonSerialized]
    public bool isInputEnd = false;
    [NonSerialized]
    public bool isEnd = false;
    [SerializeField]
    private GameObject inputEndImage;
    [NonSerialized]
    public InputEndImage inputEndImageScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
        inputEndImageScript = inputEndImage.GetComponent<InputEndImage>();
    }

    ////名前の取得用メソッド
    //public string GetName()
    //{
    //    return playerName;
    //}

    ////CallBackの取得用メソッド
    //private void doSetCallBack(FieldAudioSource audioSource)
    //{
    //    doInputKeyboardSECallBack = audioSource.doInputKeyboardSECallBack;
    //    inputEndPanelScript.doClickSECallBack = audioSource.doClickOnCallBack;
    //}

    //アドレスの入力完了パネルの初期化用メソッド
    private void InputEndInit()
    {
        inputEndImageScript.Init();
    }

    //アドレスの入力パネルの初期化用メソッド
    private void InputAddressInit()
    {
        rt.transform.localScale = Vector3.up;
        transform.gameObject.SetActive(false);
    }

    //Textの初期化用メソッド
    private void TextInit()
    {
        addressTMP.text = "";
    }

    //Flagの初期化用メソッド
    private void FlagInit()
    {
        isInputEnd = false;
        isEnd = false;
    }

    //初期設定用メソッド
    public void Init(/*FieldAudioSource inAudioSource*/)
    {
        FlagInit();
        TextInit();
        InputAddressInit();
        InputEndInit();
        //doSetCallBack(inAudioSource);
    }

    //アドレスの入力処理用メソッド
    private void InputAddressProcess(char inChar)
    {
        Keyboard keyboard = Keyboard.current;
        bool isLength = address.Length >= addressLength ? true : false;
        //文字数と入力を確認
        if (isLength ||
            keyboard.backspaceKey.isPressed ||
            keyboard.backspaceKey.wasPressedThisFrame ||
            keyboard.enterKey.isPressed ||
            keyboard.enterKey.wasPressedThisFrame ||
            keyboard.escapeKey.isPressed ||
            keyboard.escapeKey.wasPressedThisFrame) return;
        address += inChar;
        //doInputKeyboardSECallBack();
    }

    //アドレスの消去用メソッド
    private void BackSpaceAddress()
    {
        //doInputKeyboardSECallBack();
        int size = address.Length;
        //アドレスの入力数を確認
        if (size < 1) return;
        string name = null;
        for (int i = 0; i < size; i++)
        {
            //最後の文字か確認
            if (i >= size - 1) continue;
            name += address[i];
        }
        if (address.Length == 1) address = "";
        else address = name;
    }

    //キーボードの入力処理用メソッド
    private void InputKeyboard()
    {
        var keyboard = Keyboard.current;
        if (keyboard.backspaceKey.wasPressedThisFrame) BackSpaceAddress();
        keyboard.onTextInput += InputAddressProcess;
    }

    //入力可能文字数の表示用メソッド
    private void DisplayNameLength()
    {
        int nameLength = address.Length;
    }

    //入力の終了用メソッド
    private void InputEnd()
    {
        int nameLength = address.Length;
        if (nameLength == 0) return;
        if (!Keyboard.current.enterKey.wasPressedThisFrame) return;
        //doInputKeyboardSECallBack();
        isInputEnd = true;
        inputEndImage.SetActive(true);
    }

    //入力の終了確認用メソッド
    private void CheckInputEnd()
    {
        if (!isInputEnd) return;
        if (!inputEndImageScript.EasingControl() || !inputEndImageScript.isEnd) return;
        if (!EasingControl("Close")) return;
        inputEndImage.SetActive(false);
        transform.gameObject.SetActive(false);
        //dataManagerInstance.playerName = nameTMP.text;
        Keyboard.current.onTextInput -= InputAddressProcess;
        isEnd = true;
    }

    //名前の表示用メソッド
    private void DisplayName()
    {
        addressTMP.text = address;
    }

    //名前の入力用メソッド
    public void InputAddress()
    {
        DisplayName();
        CheckInputEnd();
        if (isInputEnd) return;
        InputEnd();
        DisplayNameLength();
        InputKeyboard();
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
}
