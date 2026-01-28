using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenePanel : BasePanel
{
    public Button leftBtn;
    public Button rightBtn;
    public Button beginBtn;
    public Button bakcBtn;
    public Image image;
    public Image chainImage;
    public Text sceneName;
    public Text levelId;
    public Text collectNum;
    private int nowIndex = 0;
    private bool canStart = true;

    public override void Init()
    {
        // CheckSuspendedRecord();
        GetCurrentSceneData();
        // 左按钮
        leftBtn.onClick.AddListener(() =>
         {
             ShowPrevSceneInfo();
         });
        // 右按钮
        rightBtn.onClick.AddListener(() =>
        {
            ShowNextSceneInfo();
        });
        // 开始按钮
        beginBtn.onClick.AddListener(() =>
        {
            if (nowIndex < 0 || nowIndex >= GameDataMgr.Instance.list_LevelData.Count)
            {
                Debug.Log("nowIndex 错误");
                return;
            }
            // UI处理
            UIManager.Instance.HidePanel<ScenePanel>();
            // 逻辑处理
            SceneMgr.Instance.LoadGameScene(nowIndex);
        });
        // 返回按钮
        bakcBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.HidePanel<ScenePanel>();
            UIManager.Instance.ShowPanel<BeginPanel>();
        });
    }
    public void ShowNextSceneInfo()
    {
        print("right");
        ++nowIndex;
        if (nowIndex >= GameDataMgr.Instance.list_LevelData.Count)
            nowIndex = 0;
        GetCurrentSceneData();
    }
    public void ShowPrevSceneInfo()
    {
        print("left");
        --nowIndex;
        if (nowIndex < 0)
            nowIndex = GameDataMgr.Instance.list_LevelData.Count - 1;
        GetCurrentSceneData();
    }
    public void GetCurrentSceneData()
    {
        // 显示场景信息
        LevelData sceneInfo = SceneMgr.Instance.GetSceneData(nowIndex);
        sceneName.text = sceneInfo.SceneName;
        levelId.text = "第" + sceneInfo.LevelId + 1 + "关";
        collectNum.text = "一共" + sceneInfo.TotalCollectibles + "个物品";
        image.sprite = Resources.Load<Sprite>(sceneInfo.imgRes);

        // 关卡是否解锁
        int maxUnLockedId = GameDataMgr.Instance.currentSave.maxUnlockedLevelId;
        // Debug.Log("maxUnLockedId:" + maxUnLockedId);
        // Debug.Log("nowIndex:" + nowIndex);
        if (maxUnLockedId < nowIndex)
        {
            chainImage.enabled = true;
            beginBtn.interactable = false;
        }
        else
        {
            chainImage.enabled = false;
            beginBtn.interactable = true;
        }
    }
    // 检查是否有中断记录
    public void CheckSuspendedRecord()
    {
        if (GameDataMgr.Instance.currentSave.suspendData.hasSuspendedRecord)
        {
            UIManager.Instance.ShowPanel<SuspendedPanel>();
        }
    }
}