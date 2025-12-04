using UnityEngine;
//using UnityEngine.UI;

//空腹ゲージ用スクリプトクラス
public class HungryGage : MonoBehaviour
{
    //ゲージ用変数
    private RectTransform gageRt;
    [SerializeField]
    private GameObject gageImage;
    private float maxHungry = 100.0f;
    private Transform canvas;
    [SerializeField]
    private Vector3 correctionPos;
    //表示時間管理用変数
    private const float displayTime = 5.0f;
    private float myTime = 0.0f;
    private bool displayFlag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //初期設定用メソッド
    public void Init()
    {
        gageRt = gageImage.GetComponent<RectTransform>();
        Vector2 size = new Vector2(gageRt.sizeDelta.x, maxHungry);
        gageRt.sizeDelta = size;
    }

    //表示用メソッド
    public void Display(float inValue)
    {
        gameObject.SetActive(true);
        gageRt.sizeDelta = new Vector2(gageRt.sizeDelta.x, inValue);
        myTime = 0.0f;
        displayFlag = true;
    }

    //表示時間管理用メソッド
    public void ControlDisplayTime()
    {
        if (!displayFlag) return;
        myTime += Time.deltaTime;
        if(myTime > displayTime)
        {
            myTime = 0.0f;
            gameObject.SetActive(false);
            displayFlag = false;
        }
    }

    //配置用メソッド
    public void SetPos(Transform inTransform)
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 pos = Camera.main.WorldToScreenPoint(inTransform.position);
        rt.transform.position = pos + correctionPos;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
