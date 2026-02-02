using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectMgr : MonoBehaviour
{
    private static SoundEffectMgr instance;
    public static SoundEffectMgr Instance => instance;

    private AudioSource audioSource;

    [System.Serializable]
    public class FootstepGroup
    {
        public string materialName;
        public List<AudioClip> clips;
    }
    private int currentStepIndex = 0;
    private string lastMaterial = "";
    public List<FootstepGroup> footstepGroups;
    private Dictionary<string, List<AudioClip>> footstepDict = new Dictionary<string, List<AudioClip>>();
    void Awake()
    {
        instance = this;
        audioSource = this.GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        // 将 List 转为 Dictionary 方便快速查询
        foreach (var group in footstepGroups)
        {
            footstepDict[group.materialName] = group.clips;
        }
        // 初始化数据
        MusicData musicData = GameDataMgr.Instance.musicDatas;
        SetIsOpen(musicData.effectOpen);
        SetVolume(musicData.effectValue);
    }

    // 播放音效的方法
    public void PlaySound(string name, bool isAsync = false)
    {
        // 路径根据你的资源存放位置修改，这里假设在 Resources/Sounds/ 下
        AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/" + name + ".wav");
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("asdasdasdasd");
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
    public void PlayFootstep(string materialName)
    {
        if (!footstepDict.ContainsKey(materialName)) return;

        // 如果材质变了，或者索引超过范围，从第一个开始
        if (lastMaterial != materialName)
        {
            ResetFootsteps();
            lastMaterial = materialName;
        }

        List<AudioClip> clips = footstepDict[materialName];
        if (clips.Count == 0) return;

        // 播放当前索引的音效
        audioSource.PlayOneShot(clips[currentStepIndex]);

        // 顺序增加索引，循环往复
        currentStepIndex = (currentStepIndex + 1) % clips.Count;
    }
    public void ResetFootsteps()
    {
        currentStepIndex = 0;
    }
}