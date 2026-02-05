using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class BasePanel : MonoBehaviour
{
    //专门控制面板的透明度的组件
    CanvasGroup canvasGroup;
    //淡入淡出的速度
    float alphaSpeed = 10;
    //是否显示
    bool isShow = false;
    UnityAction hideCallBack = null;

    /// <summary>
    /// 此面板显示时是否需要锁定玩家输入（移动、跳跃、技能等）
    /// 子面板可重写此属性来控制输入锁定行为
    /// </summary>
    protected virtual bool LockPlayerInputOnShow => false;

    /// <summary>
    /// 是否锁定玩家输入（供UIManager使用）
    /// </summary>
    public bool ShouldLockPlayerInput => LockPlayerInputOnShow;

    /// <summary>
    /// 注册控件事件
    /// </summary>
    public abstract void Init();
    protected virtual void Start()
    {
        Init();
    }
    protected virtual void Awake()
    {
        //一开始获取面板上的canvasGroup
        if (canvasGroup == null)
            this.AddComponent<CanvasGroup>();
        canvasGroup = this.GetComponent<CanvasGroup>();

    }
    public virtual void ShowMe()
    {
        canvasGroup.alpha = 0;
        isShow = true;
    }
    public virtual void HideMe(UnityAction callBack)
    {
        canvasGroup.alpha = 1;
        isShow = false;
        hideCallBack = callBack;
    }
    void Update()
    {
        if (isShow && canvasGroup.alpha != 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime * alphaSpeed;
            if (canvasGroup.alpha >= 1)
                canvasGroup.alpha = 1;
        }
        if (!isShow && canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime * alphaSpeed;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                hideCallBack?.Invoke();
            }
        }

    }
}
