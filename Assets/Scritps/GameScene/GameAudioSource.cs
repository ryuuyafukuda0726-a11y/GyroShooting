using UnityEngine;
using System;

//BGMの種類
public enum GameBGM
{
    Main
}

//SEの種類
public enum GameSE
{
    Shot,
    Shot_Hit
}

//GameSceneのAudioSource用スクリプトクラス
public class GameAudioSource : MonoBehaviour
{
    //AudioSource用変数
    private AudioSource audioSource;
    //BGM用変数
    [SerializeField]
    private AudioClip[] bgm = new AudioClip[Enum.GetValues(typeof(GameBGM)).Length];
    //SE用変数
    [SerializeField]
    private AudioClip[] se = new AudioClip[Enum.GetValues(typeof(GameSE)).Length];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    //引数に対応したBGMの再生用メソッド
    public void PlayBGM(int inBGMNumber)
    {
        audioSource.clip = bgm[inBGMNumber];
        audioSource.Play();
        audioSource.loop = true ;
    }

    //引数に対応したSEを再生するコールバック用メソッド
    public void PlaySECallBack(int inSENumber)
    {
        audioSource.PlayOneShot(se[inSENumber]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
