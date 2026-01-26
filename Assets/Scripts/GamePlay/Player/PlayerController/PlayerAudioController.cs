using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("音效库")]//这里存放我们准备好的音效，它们有的会用在anim event中，有的会用在状态机中
    
    //比如说，如下
    public AudioClip jumpSound;
    //public AudioClip landSound;
    //public AudioClip dashSound;
    //public AudioClip dieSound;
    //public AudioClip walkStepSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 播放单次音效函数
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }

    // 快捷方法供状态机调用，举个跳跃的例子
    public void PlayJump() => PlaySFX(jumpSound);
}
