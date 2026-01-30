using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{

    // public Button backBtn;
    public Button settingBtn;
    public override void Init()
    {
        // 设置按钮
        settingBtn.onClick.AddListener(() =>
        {
            // UI逻辑
            UIManager.Instance.ShowPanel<SettingPanel>();
            UIManager.Instance.GetPanel<SettingPanel>().ShowBtn();
        });

    }

}
