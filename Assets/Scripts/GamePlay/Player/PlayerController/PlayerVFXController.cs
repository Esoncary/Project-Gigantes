using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXController : MonoBehaviour
{
    [Header("引用子物体特效")]
    //跑步回城
    public Animator VFXRunDustAnimator;
    public SpriteRenderer VFXRunDustRenderer; // 如果需要控制显示/隐藏

    //死亡特效
    public Animator VFXDieAnimator;
    public SpriteRenderer VFXDieRenderer; // 如果需要控制显示/隐藏

    //落地特效
    public Animator VFXLandAnimator;
    public SpriteRenderer VFXLandRenderer;

    //跳跃特效
    public Animator VFXJumpAnimator;
    public SpriteRenderer VFXJumpRenderer;





    private void Awake()
    {
        // 初始状态通常是关闭的
        if (VFXDieRenderer != null) VFXDieRenderer.enabled = false;
        if (VFXRunDustRenderer != null) VFXRunDustRenderer.enabled = false;
        if (VFXJumpRenderer != null) VFXJumpRenderer.enabled = false;
    }

    // 启动跑步灰尘
    public void PlayRunDust()
    {
        // Debug.Log("成功播放");

        // 1. 开启渲染器显示
        if (VFXRunDustRenderer != null) VFXRunDustRenderer.enabled = true;

        // 2. 播放指定名字的动画
        // 这里的 "Run_Dust_Anim" 必须和你子物体 Animator 里的状态名一致
        VFXRunDustAnimator.Play("VFXRunDust", 0, 0f); // 最后一个参数 0f 表示从第一帧开始重头播
    }
    public void StopRunDust()
    {
        if (VFXRunDustRenderer != null) VFXRunDustRenderer.enabled = false;

    }

    //启动死亡特效
    public void PlayDie()
    {
        // 1. 开启渲染器显示
        if (VFXDieRenderer != null) VFXDieRenderer.enabled = true;
        // 2. 播放指定名字的动画
        // 这里的 "Die_Anim" 必须和你子物体 Animator 里的状态名一致
        VFXDieAnimator.Play("VFXDie", 0, 0f); // 最后一个参数 0f 表示从第一帧开始重头播
    }
    //结束播放在它自身的脚本上


    //落地特效
    public void PlayLand()
    {
        // 1. 开启渲染器显示
        if (VFXLandRenderer != null) VFXLandRenderer.enabled = true;
        // 2. 播放指定名字的动画
        // 这里的 "Land_Anim" 必须和你子物体 Animator 里的状态名一致
        VFXLandAnimator.Play("VFXLand", 0, 0f); // 最后一个参数 0f 表示从第一帧开始重头播
    }
    //结束播放的脚本在它自己身上
}
