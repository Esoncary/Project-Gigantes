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

    private string currentStopableSoundName;
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

        currentStopableSoundName = string.Empty;
    }

    // 播放音效的方法
    public void PlaySound(string name, bool isStopable = false)
    {
        // 路径根据你的资源存放位置修改，这里假设在 Resources/Sounds/ 下
        AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/SFX/" + name + ".wav");
        if (clip != null)
        {
            if (isStopable)
            {
                // 逻辑：可停止音效 - 赋值clip+播放，记录名称
                currentStopableSoundName = name;
                audioSource.clip = clip;
                audioSource.Play();
                audioSource.loop = true;
            }
            else
            {
                // 保留原有逻辑：不可停止的一次性短音效
                audioSource.PlayOneShot(clip);
            }
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
    public void StopSound(string soundName)
    {
        // 仅当目标音效是当前正在播放的可停止音效时，执行停止
        if (currentStopableSoundName == soundName && audioSource.isPlaying)
        {
            audioSource.Stop();
            currentStopableSoundName = string.Empty;
        }
    }
}