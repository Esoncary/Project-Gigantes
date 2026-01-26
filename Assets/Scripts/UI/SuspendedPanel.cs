using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuspendedPanel : BasePanel
{

    public Button confirmBtn;
    public Button cancelBtn;
    public override void Init()
    {
        confirmBtn.onClick.AddListener(() =>
        {
            PlayerSaveData data = GameDataMgr.Instance.currentSave;
            int levelId = data.SuspendLevelId;
            SceneMgr.Instance.LoadScene(levelId, data.HasSuspendedRecord);

            UIManager.Instance.HidePanel<SuspendedPanel>();
            UIManager.Instance.HidePanel<ScenePanel>();
        });
        cancelBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<SuspendedPanel>();
            GameDataMgr.Instance.ClearSuspendData();
        });

    }

}
