using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{

    public Button backBtn;
    public Button settingBtn;
    public override void Init()
    {

        backBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.ShowPanel<ConfirmPanel>();
        });
        settingBtn.onClick.AddListener(() =>
        {
            // UIManager.Instance.HidePanel<GamePanel>();
            UIManager.Instance.ShowPanel<SettingPanel>();
        });

    }

}
