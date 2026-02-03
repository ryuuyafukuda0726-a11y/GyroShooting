using UnityEngine;
using TMPro;

//タイマー用スクリプトクラス
public class Timer : MonoBehaviour
{
    //タイマー用変数
    private const float minutes = 60.0f;
    private float timer = 0.0f;
    //テキスト用変数
    [SerializeField]
    private TextMeshProUGUI timerTMP;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //初期設定用メソッド
    public void Init(float inTime)
    {
        timer = inTime;
    }

    //時間の表示用メソッド
    private void DisplayTimer()
    {
        string minutsString = "" + (int)(timer / minutes);
        string secondsString = "" + (int)(timer % minutes);
        string time = minutsString + ":" + secondsString;
        timerTMP.text = time;
    }

    //タイマーカウント用メソッド
    public bool TimerCount()
    {
        timer -= Time.deltaTime;
        if(timer < 0.0f)
        {
            timer = 0.0f;
            return true;
        }
        DisplayTimer();
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
