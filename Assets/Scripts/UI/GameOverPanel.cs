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
        // 下一关按钮
        nextLevelBtn.onClick.AddListener(async () =>
        {
            int levelId = SceneMgr.Instance.GetSceneIdByName("UIScene");
            // Debug.Log(levelId);
            SceneMgr.Instance.LoadSceneAsync(levelId, () =>
            {
                UIManager.Instance.HidePanel<GamePanel>();
                UIManager.Instance.HidePanel<BeginPanel>();
                UIManager.Instance.HidePanel<GameOverPanel>();
                UIManager.Instance.ShowPanel<ScenePanel>();
                UIManager.Instance.GetPanel<ScenePanel>().ShowNextSceneInfo();
            });
        });
        // 重新开始按钮
        restartBtn.onClick.AddListener(() =>
        {
            //UI处理
            UIManager.Instance.HidePanel<GameOverPanel>();
            // 逻辑处理
            SceneMgr.Instance.TriggerReload();
        });
        // 主菜单按钮
        mainMenuBtn.onClick.AddListener(async () =>
        {
            int levelId = SceneMgr.Instance.GetSceneIdByName("UIScene");
            // Debug.Log(levelId);
            SceneMgr.Instance.LoadSceneAsync(levelId, () =>
        {
            UIManager.Instance.HidePanel<GamePanel>();
            UIManager.Instance.HidePanel<ScenePanel>();
            UIManager.Instance.HidePanel<GameOverPanel>();
        });

        });
    }
    public override void ShowMe()
    {
        base.ShowMe();
        GameDataMgr.Instance.SaveLevelComplete(100, 0);
        Time.timeScale = 0;
    }
    public override void HideMe(UnityAction callBack)
    {
        base.HideMe(callBack);
        Time.timeScale = 1;
    }

}
