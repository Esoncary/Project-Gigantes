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
        confirmBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ConfirmPanel>();
            UIManager.Instance.HidePanel<GamePanel>();
            UIManager.Instance.HidePanel<SettingPanel>();
            SceneManager.LoadScene("UIScene");

            GameDataMgr.Instance.SaveSuspendData(SceneMgr.Instance.currentSceneId, SceneMgr.Instance.playerController.transform.position);
        });
        cancelBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ConfirmPanel>();
        });

    }

}
