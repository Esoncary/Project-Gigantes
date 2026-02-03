using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestionPanel : BasePanel
{

    public Button closeBtn;
    public override void Init()
    {
        // 确认退出游戏按钮
        closeBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.HidePanel<QuestionPanel>();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });

    }

}
