using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BkgMusicMgr : MonoBehaviour
{
    AudioSource audioSource;

    private static BkgMusicMgr instance;
    public static BkgMusicMgr Instance => instance;
    void Awake()
    {
        instance = this;
        audioSource = this.GetComponent<AudioSource>();
        MusicData musicData = GameDataMgr.Instance.musicDatas;
        SetIsOpen(musicData.musicOpen);
        SetVolume(musicData.musicValue);
    }
    public void SetIsOpen(bool value)
    {
        audioSource.mute = !value;
    }
    public void SetVolume(float value)
    {
        audioSource.volume = value;
    }
}
