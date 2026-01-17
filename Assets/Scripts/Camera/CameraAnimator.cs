using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraAnimator : MonoBehaviour
{
    Animator animator;
    UnityAction action;
    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }
    //左转函数
    public void TurnLeft(UnityAction e)
    {
        animator.SetTrigger("Left");
        action += e;
    }
    //右转函数
    public void TurnRight(UnityAction e)
    {
        animator.SetTrigger("Right");
        action += e;
    }
    //动画播放完成
    public void PlayOver()
    {
        action?.Invoke();
        action = null;
    }
}
