using UnityEngine;

//プレイモード選択画面用スクリプトクラス
public class PlayModeImage : MonoBehaviour
{
    //イージング用変数
    private EasingControl easing;
    private Vector3 aVec, bVec;
    private float percent = 0.0f;
    private const float maxPercent = 1.0f;
    private const float minPercent = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //イージング管理用メソッド
    public bool EasingControl(string inMove)
    {
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
