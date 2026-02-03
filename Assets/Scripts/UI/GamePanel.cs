using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{

    // public Button backBtn;
    public Button settingBtn;
    public Button questionBtn;
    public Text scoreText;
    public Text totalScoreText;
    public override void Init()
    {
        totalScoreText.text = "/" + GameDataMgr.Instance.list_LevelData[GameDataMgr.Instance.currentLevelId].TotalCollectibles.ToString();
        scoreText.text = GameDataMgr.Instance.currentLevelCollectedIds.Count.ToString();
        // 设置按钮
        settingBtn.onClick.AddListener(() =>
        {
            // UI逻辑
            UIManager.Instance.ShowPanel<SettingPanel>();
            UIManager.Instance.GetPanel<SettingPanel>().ShowBtn();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
        questionBtn.onClick.AddListener(() =>
        {
            // UI逻辑
            UIManager.Instance.ShowPanel<QuestionPanel>();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });

    }
    public void RefreshKeyUI()
    {
        // 空值防护
        if (scoreText == null) return;

        // 从存档中读取钥匙数量，更新文本
        int keyCount = GameDataMgr.Instance.currentLevelCollectedIds.Count;
        scoreText.text = keyCount.ToString();
    }
}
