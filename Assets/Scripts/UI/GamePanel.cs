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
    public Image currentStorage;
    public Image targetStorage;
    public Image bkgStorage;
    public Image gas;
    public Animator anim;
    public override void Init()
    {
        ChangeStorageUI(true);
        totalScoreText.text = "/" + GameDataMgr.Instance.list_LevelData[GameDataMgr.Instance.currentLevelId].TotalCollectibles.ToString();
        scoreText.text = GameDataMgr.Instance.currentLevelCollectedIds.Count.ToString();
        // anim = gas.GetComponent<Animator>();
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
    public void ChangeStorageUI(bool isBorken)
    {
        // 空值防护
        if (currentStorage == null || targetStorage == null) return;
        Sprite bkgSprite;
        Sprite progressSprite;
        if (!isBorken)
        {
            bkgSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/进度条裂开底框.png");
            progressSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/灰色进度条.png");
            // gas.gameObject.SetActive(false);
            targetStorage.type = Image.Type.Sliced;
            currentStorage.type = Image.Type.Sliced;
            anim.SetBool("overload", true);
        }
        else
        {
            bkgSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/红色阀门进度条_槽.png");
            progressSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/颜色进度条.png");
            // gas.gameObject.SetActive(true);
            targetStorage.type = Image.Type.Filled;
            currentStorage.type = Image.Type.Filled;
            anim.SetBool("overload", false);
        }
        bkgStorage.sprite = bkgSprite;
        targetStorage.sprite = progressSprite;
        currentStorage.sprite = progressSprite;
    }
}
