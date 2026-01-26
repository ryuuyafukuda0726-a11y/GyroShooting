using UnityEngine;

//マッチング部屋用スクリプトクラス
public class MatchingRoomImage : MonoBehaviour
{
    //レクトトランスフォーム用変数
    private RectTransform rt;
    //イージング用変数
    private EasingControl easing;
    private Vector3 aVec, bVec;
    private float percent = 0.0f;
    private const float minPercent = 0.0f;
    private const float maxPercent = 1.0f;
    //マッチング時間用変数
    [SerializeField]
    private GameObject matchingTimeImage;
    [SerializeField]
    private float matchingTime = 0.0f;
    //マッチングプレイヤー用変数
    [SerializeField]
    private GameObject[] matchingPlayers = new GameObject[4];
    //プレイマップ用変数
    [SerializeField]
    private GameObject playMapImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    //初期設定用メソッド
    public void Init()
    {
        rt.transform.localScale = Vector3.up;
        easing = global::EasingControl.SetEasing;
    }

    //マップ写真と説明の表示用メソッド
    private void DisplayMapPictureAndExplanation()
    {

    }

    //プレイヤーの表示用メソッド
    private void DisplayPlayer()
    {

    }

    //タイマー表示用メソッド
    private void DisplayTimer()
    {

    }

    //プレイ用メソッド
    public void Play()
    {

    }

    //イージング設定用メソッド
    private void SetEasing(string inMove)
    {
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
        //イージングの進行状態でスイッチ
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

    //戻るボタン用メソッド
    public void ReturnButton()
    {

    }

    //ゲーム開始ボタン用メソッド
    public void GameStartButton()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
