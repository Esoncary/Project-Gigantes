using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button startBtn;
    public Button settingBtn;
    public Button quitBtn;
    public override void Init()
    {
        // 开始按钮
        startBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.HidePanel<BeginPanel>();
            UIManager.Instance.ShowPanel<ScenePanel>();

            // 默认选择存档1
            
            GameDataMgr.Instance.LoadChoosePlayerSaveData(0);
            GameDataMgr.Instance.currentSave.ToString();
        });

        // 设置按钮
        settingBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.ShowPanel<SettingPanel>();
            UIManager.Instance.GetPanel<SettingPanel>().HideBtn();
        });

        // 退出按钮
        quitBtn.onClick.AddListener(() =>
        {
            GameDataMgr.Instance.SavePlayerSaveData();
            Application.Quit();
        });
    }
}
