using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;

public class SceneMgr
{
    public List<LevelData> sceneInfos;

    private static SceneMgr instance = new SceneMgr();
    public static SceneMgr Instance => instance;
    private SceneMgr()
    {
        sceneInfos = GameDataMgr.Instance.list_LevelData;
    }
    // private PlayerObj playerObj;
    public void InitInfo()
    {
        // UI显示
        UIManager.Instance.ShowPanel<GamePanel>();

        // 角色加载
        // Transform playerPos = GameObject.Find("PlayerPos").transform;
        // GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(roleInfo.res), playerPos.position, playerPos.rotation, playerPos);
        // playerObj = obj.GetComponent<PlayerObj>();
        // playerObj.InitPlayerInfo(roleInfo.atk, GameDataMgr.Instance.sceneDatas[GameDataMgr.Instance.nowSceneIndex].money);
    }

    // 加载场景数据
    public LevelData GetSceneData(int index)
    {
        LevelData sceneInfo = sceneInfos[index];
        return sceneInfo;
    }

    // 加载场景
    public void LoadScene(int index)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneInfos[index].LevelId);

        ao.completed += (obj) =>
        {
            InitInfo();
        };
    }

    // 通关自动保存
    public void PassAndAutoSave(int index)
    {
        GameDataMgr.Instance.currentSave.MaxUnlockedLevelId++;
        GameDataMgr.Instance.SavePlayerSaveData();
    }

}
