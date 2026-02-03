using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 自定义带音效的按钮，继承原生UGUI Button
/// </summary>
public class SoundButton : Button
{
    /// <summary>
    /// 重写 点击/确认 交互方法（覆盖原生触发逻辑）
    /// 支持：鼠标点击、键盘回车、控制器导航确认等所有触发方式
    /// </summary>
    public override void OnSubmit(BaseEventData eventData)
    {
        // 先执行原生Button的逻辑（关闭/跳转等）
        base.OnSubmit(eventData);
        // 播放点击音效
        PlayClickSound();
    }

    /// <summary>
    /// 重写 指针点击方法（适配鼠标/触摸点击）
    /// </summary>
    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        PlayClickSound();
    }

    /// <summary>
    /// 统一播放音效逻辑
    /// </summary>
    private void PlayClickSound()
    {
        // 防护：按钮不可交互时不播放音效 + 管理器判空
        if (!interactable || SoundEffectMgr.Instance == null) return;

        // 调用全局默认按钮音效
        SoundEffectMgr.Instance.PlaySound("UI/button_click");
    }
}