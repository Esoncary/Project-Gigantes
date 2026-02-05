using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneNamePanel : BasePanel
{
    public Text text;
    public override void Init()
    {
        text.text = SceneMgr.Instance.GetSceneData(GameDataMgr.Instance.currentLevelId).LevelName;
    }
    public async Task ShowAsync()
    {
        // 校验面板对象是否存活，防止销毁后调用报错
        if (this == null || gameObject == null) return;

        // 执行基类显示逻辑
        base.ShowMe();

        // 等待2秒真实时间（无视游戏暂停/倍速，适合UI计时）
        await Task.Delay(2000);

        // 再次校验，延迟期间面板可能被销毁
        if (this == null || gameObject == null) return;

        // 执行基类隐藏逻辑
        base.HideMe(null);
    }

    // 兼容原有同步调用（可选保留，不影响新逻辑）
    public void Show()
    {
        // 丢弃异步任务，仅做兼容
        _ = ShowAsync();
    }
}
