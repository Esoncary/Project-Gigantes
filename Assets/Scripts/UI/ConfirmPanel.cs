using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmPanel : BasePanel
{

    public Button confirmBtn;
    public Button cancelBtn;
    public override void Init()
    {
        // 确认退出游戏按钮
        confirmBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.HidePanel<ConfirmPanel>();
            UIManager.Instance.HidePanel<GamePanel>();
            UIManager.Instance.HidePanel<SettingPanel>();
            // 逻辑处理
            // SceneMgr.Instance.UpdateCheckpoint(SceneMgr.Instance.currentRebornPos);
            SceneMgr.Instance.LoadSceneAsync(SceneMgr.Instance.GetSceneIdByName("UIScene"));

        });

        cancelBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.HidePanel<ConfirmPanel>();
        });

    }

}
