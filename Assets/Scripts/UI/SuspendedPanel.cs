using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuspendedPanel : BasePanel
{
    // 中断记录面板需要锁定玩家输入
    protected override bool LockPlayerInputOnShow => true;

    public Button confirmBtn;
    public Button cancelBtn;
    public override void Init()
    {
        // 继续中断按钮
        confirmBtn.onClick.AddListener(() =>
        {
            // Ui处理
            UIManager.Instance.HidePanel<SuspendedPanel>();
            UIManager.Instance.HidePanel<ScenePanel>();
            // 逻辑处理
            int levelId = GameDataMgr.Instance.currentSave.suspendData.suspendLevelId;
            SceneMgr.Instance.LoadGameScene(levelId);
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
        // 取消中断按钮
        cancelBtn.onClick.AddListener(() =>
        {
            // Ui处理
            UIManager.Instance.HidePanel<SuspendedPanel>();
            // 逻辑处理
            GameDataMgr.Instance.ClearSuspendData();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });

    }

}
