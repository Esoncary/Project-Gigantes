using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : BasePanel
{
    public Button nextLevelBtn;
    public Button restartBtn;
    public Button mainMenuBtn;
    public override void Init()
    {
        //初始时 得到存储的数据
        MusicData musicData = GameDataMgr.Instance.musicDatas;
        GameDataMgr.Instance.ClearSuspendData();
        nextLevelBtn.onClick.AddListener(async () =>
        {
            int levelId = SceneMgr.Instance.GetSceneBuildIndex("UIScene");
            Debug.Log(levelId);
            await SceneMgr.Instance.ReloadCurrentLevelAsync(levelId, () =>
        {
            UIManager.Instance.HidePanel<GamePanel>();
            UIManager.Instance.HidePanel<BeginPanel>();
            return Task.CompletedTask;
        });
        });
        restartBtn.onClick.AddListener(() =>
        {
            SceneMgr.Instance.TriggerReload();
            UIManager.Instance.HidePanel<GameOverPanel>();
        });
        mainMenuBtn.onClick.AddListener(async () =>
        {
            int levelId = SceneMgr.Instance.GetSceneBuildIndex("UIScene");
            Debug.Log(levelId);
            await SceneMgr.Instance.ReloadCurrentLevelAsync(levelId, () =>
        {
            UIManager.Instance.HidePanel<GamePanel>();
            UIManager.Instance.HidePanel<ScenePanel>();
            return Task.CompletedTask;
        });

        });
    }
    public override void ShowMe()
    {
        base.ShowMe();
        Time.timeScale = 0;
    }
    public override void HideMe(UnityAction callBack)
    {
        base.HideMe(callBack);
        Time.timeScale = 1;
    }

}
