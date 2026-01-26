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
    public Text sceneName;
    public Text levelId;
    public Text collectNum;
    private int nowIndex = 0;

    public override void Init()
    {
        CheckSuspendedRecord();
        GetCurrentSceneData();
        leftBtn.onClick.AddListener(() =>
         {
             print("left");
             --nowIndex;
             if (nowIndex < 0)
                 nowIndex = GameDataMgr.Instance.list_LevelData.Count - 1;
             //  Debug.Log(GameDataMgr.Instance.list_LevelData.Count);
             GetCurrentSceneData();
         });
        rightBtn.onClick.AddListener(() =>
        {
            print("right");
            ++nowIndex;
            if (nowIndex >= GameDataMgr.Instance.list_LevelData.Count)
                nowIndex = 0;
            GetCurrentSceneData();
        });
        beginBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ScenePanel>();
            SceneMgr.Instance.LoadScene(nowIndex);
        });
        bakcBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ScenePanel>();
            UIManager.Instance.ShowPanel<BeginPanel>();
        });
    }
    public void GetCurrentSceneData()
    {
        LevelData sceneInfo = SceneMgr.Instance.GetSceneData(nowIndex);
        sceneName.text = sceneInfo.SceneName;
        levelId.text = "第" + sceneInfo.LevelId + "关";
        collectNum.text = "一共" + sceneInfo.TotalCollectibles + "个物品";
        image.sprite = Resources.Load<Sprite>(sceneInfo.imgRes);
    }
    // 检查是否有中断记录
    public void CheckSuspendedRecord()
    {
        // Debug.Log(GameDataMgr.Instance.currentSave);
        if (GameDataMgr.Instance.currentSave.HasSuspendedRecord)
        {
            // 显示是否继续面板
            UIManager.Instance.ShowPanel<SuspendedPanel>();
        }
        // UIManager.Instance.ShowPanel<SuspendedPanel>();
    }
}