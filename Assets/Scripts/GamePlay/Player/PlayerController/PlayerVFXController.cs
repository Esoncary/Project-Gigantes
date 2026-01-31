using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXController : MonoBehaviour
{
    [Header("引用子物体特效")]
    //跑步灰尘
    public Animator VFXRunDustAnimator;
    public SpriteRenderer VFXRunDustRenderer; // 如果需要控制显示/隐藏

    //过载喷气
    public Animator VFXOverloadAnimator;
    public SpriteRenderer VFXOverloadRenderer;

    //冷却剂特效
    public Animator VFXCooledAnimator;
    public SpriteRenderer VFXCooledRenderer;

    //增强剂特效
    public Animator VFXStrengthened;
    public SpriteRenderer VFXStrengthenedRenderer;

    //释放特效
    public Animator VFXReleaseAnimator;
    public SpriteRenderer VFXReleaseRenderer;

    //死亡特效
    public GameObject VFXDie;

    //落地特效
    public GameObject VFXLandDust;
    public Animator VFXLandAnimator;
    public SpriteRenderer VFXLandRenderer;

    //跳跃灰尘特效
    public Animator VFXJumpDustAnimator;
    public SpriteRenderer VFXJumpDustRenderer;






    private void Awake()
    {
        // 初始状态通常是关闭的
        if (VFXOverloadRenderer != null) VFXOverloadRenderer.enabled = false; 
        if (VFXRunDustRenderer != null) VFXRunDustRenderer.enabled = false;
        if (VFXCooledRenderer != null) VFXCooledRenderer.enabled = false;
        if (VFXStrengthenedRenderer != null) VFXStrengthenedRenderer.enabled = false;
        if (VFXReleaseRenderer != null) VFXReleaseRenderer.enabled = false;
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
    public void PlayDie(Transform playerPos)
    {
        Instantiate(VFXDie, playerPos.position, Quaternion.identity);
    }
    //结束播放在它自身的脚本上


    //落地特效
    public void PlayLandDust(Transform playerPos)
    {
        Instantiate(VFXLandDust, playerPos.position, Quaternion.identity);//其实这里我也不清楚，为什么角色的位置正好是脚下，应该是图片自身的pivot就比较低
    }
    //结束播放的脚本在它自己身上

    //跳跃灰尘特效（和上面位置不一样）
    public void PlayJumpDust(Transform playerPos)
    {
        Vector3 spawnPos = new Vector3 (playerPos.position.x, playerPos.position.y - 1f, playerPos.position.z);
        
        Instantiate(VFXLandDust, spawnPos, Quaternion.identity);
    }

    //启动过载喷气特效
    public void PlayeOverload()
    {
        // 1. 开启渲染器显示
        if (VFXOverloadRenderer != null) VFXOverloadRenderer.enabled = true;

        // 2. 播放指定名字的动画
        // 这里的 "Run_Dust_Anim" 必须和你子物体 Animator 里的状态名一致
        VFXOverloadAnimator.Play("VFXOverload", 0, 0f); // 最后一个参数 0f 表示从第一帧开始重头播
    }
    public void StopOverload()
    {
        if (VFXOverloadRenderer != null) VFXOverloadRenderer.enabled = false;

    }

    //启动冷却剂特效
    public void PlayCooled()
    {
        // 1. 开启渲染器显示
        if (VFXCooledRenderer != null) VFXCooledRenderer.enabled = true;
        // 2. 播放指定名字的动画
        // 这里的 "Run_Dust_Anim" 必须和你子物体 Animator 里的状态名一致
        VFXCooledAnimator.Play("VFXCooled", 0, 0f); // 最后一个参数 0f 表示从第一帧开始重头播
    }
    public void StopCooled()
    {
        if (VFXCooledRenderer != null) VFXCooledRenderer.enabled = false;
    }

    //启动增强剂特效
    public void PlayStrengthened()
    {
        // 1. 开启渲染器显示
        if (VFXStrengthenedRenderer != null) VFXStrengthenedRenderer.enabled = true;
        // 2. 播放指定名字的动画
        // 这里的 "Run_Dust_Anim" 必须和你子物体 Animator 里的状态名一致
        VFXStrengthened.Play("VFXStrengthened", 0, 0f); // 最后一个参数 0f 表示从第一帧开始重头播
    }
    public void StopStrengthened()
    {
        if (VFXStrengthenedRenderer != null) VFXStrengthenedRenderer.enabled = false;
    }


    //启用释放特效
    public void PlayRelease(float angle)
    {
        //1.角度设置
        VFXReleaseRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
        // 2. 开启渲染器显示
        if (VFXReleaseRenderer != null) VFXReleaseRenderer.enabled = true;
        // 3. 播放指定名字的动画
        VFXReleaseAnimator.Play("VFXRelease", 0, 0f); // 最后一个参数 0f 表示从第一帧开始重头播
    }
}
