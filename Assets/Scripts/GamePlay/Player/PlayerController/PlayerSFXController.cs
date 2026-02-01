using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFXController : MonoBehaviour
{
    [Header("核心组件")]
    //这两个组件，都是挂在玩家身上的
    public AudioSource sfxSource;       // 用于播放瞬发音效（跳跃、落地等）
    public AudioSource loopingSource;   // 用于播放持续音效（过载警告、跑步声等）

    [Header("移动音效库")]
    public AudioClip sfxJump;
    public AudioClip sfxLand;
    public AudioClip sfxRunStep;        // 如果通过动画事件触发，建议用这个

    [Header("动力系统音效")]
    public AudioClip sfxOverloadWarning; // 过载循环音
    public AudioClip sfxRelease;         // 释放/冲刺瞬发音
    public AudioClip sfxDeadExplosion;   // 死亡爆炸

    [Header("道具音效")]
    public AudioClip sfxCooled;
    public AudioClip sfxStrengthened;

    private void Awake()
    {
        // 自动获取或初始化组件
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();

        // 设置初始属性
        if (sfxSource != null) sfxSource.playOnAwake = false;
        if (loopingSource != null)
        {
            loopingSource.playOnAwake = false;
            loopingSource.loop = true;
        }
    }

    // --- 移动类音效 ---

    // 播放跳跃音效
    public void PlayJumpSFX()
    {
        PlayRandomPitch(sfxJump, 1.0f);
    }

    // 播放落地音效
    public void PlayLandSFX()
    {
        PlayRandomPitch(sfxLand, 0.8f);
    }

    // 播放跑步脚步声 (通常由动画事件触发)
    public void PlayRunStepSFX()
    {
        PlayRandomPitch(sfxRunStep, 0.5f);
    }

    // --- 动力系统音效 ---

    // 开启过载警告音（循环）
    public void StartOverloadSFX()
    {
        if (loopingSource != null && sfxOverloadWarning != null)
        {
            if (!loopingSource.isPlaying || loopingSource.clip != sfxOverloadWarning)
            {
                loopingSource.clip = sfxOverloadWarning;
                loopingSource.Play();
            }
        }
    }

    // 停止过载警告音
    public void StopOverloadSFX()
    {
        if (loopingSource != null && loopingSource.clip == sfxOverloadWarning)
        {
            loopingSource.Stop();
        }
    }

    // 播放释放/冲刺音效
    public void PlayReleaseSFX()
    {
        PlayRandomPitch(sfxRelease, 1.1f);
    }

    // 播放死亡爆炸音效
    public void PlayDieSFX()
    {
        // 死亡通常是大声且不改变音高的
        if (sfxSource != null && sfxDeadExplosion != null)
        {
            sfxSource.PlayOneShot( sfxDeadExplosion, 1.2f);
        }
    }

    // --- 道具类音效 ---

    public void PlayCooledSFX()
    {
        PlayRandomPitch(sfxCooled, 1.0f);
    }

    public void PlayStrengthenedSFX()
    {
        PlayRandomPitch(sfxStrengthened, 1.0f);
    }

    // --- 内部辅助工具 ---

    /// <summary>
    /// 播放随机音高的音效，能有效防止玩家听觉疲劳，增加“打击感”
    /// </summary>
    private void PlayRandomPitch(AudioClip clip, float volume)
    {
        if (sfxSource != null && clip != null)
        {
            // 在 0.9 到 1.1 之间随机变调，让每次跳跃或跑步声音都有细微差别
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(clip, volume);
            // 播放后重置回标准音高以免影响其他逻辑
            sfxSource.pitch = 1.0f;
        }
    }
}