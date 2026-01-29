using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectMgr : MonoBehaviour
{
    private static SoundEffectMgr instance;
    public static SoundEffectMgr Instance => instance;

    private AudioSource audioSource;

    void Awake()
    {
        instance = this;
        // 动态添加AudioSource，或者手动挂载
        audioSource = this.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // 初始化数据
        MusicData musicData = GameDataMgr.Instance.musicDatas;
        SetIsOpen(musicData.effectOpen);
        SetVolume(musicData.effectValue);
    }

    // 播放音效的方法
    public void PlaySound(string name, bool isAsync = false)
    {
        // 路径根据你的资源存放位置修改，这里假设在 Resources/Sounds/ 下
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + name);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("未找到音效文件: " + name);
        }
    }

    // 设置是否开启
    public void SetIsOpen(bool value)
    {
        audioSource.mute = !value;
    }

    // 设置音量
    public void SetVolume(float value)
    {
        audioSource.volume = value;
    }
}