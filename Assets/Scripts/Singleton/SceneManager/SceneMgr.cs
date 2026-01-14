using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;

public class SceneMgr
{
    public List<SceneData> sceneInfos;

    private static SceneMgr instance = new SceneMgr();
    public static SceneMgr Instance => instance;
    private SceneMgr()
    {
        foreach (SceneData sceneData in GameDataMgr.Instance.sceneDatas)
        {
            sceneInfos.Add(sceneData);
        }
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
    public SceneData GetSceneData(int index)
    {
        SceneData sceneInfo = sceneInfos[index];
        return sceneInfo;
    }
    // 加载场景
    public void LoadScene(int index)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneInfos[index].id);

        ao.completed += (obj) =>
        {
            InitInfo();
        };
    }
    //判断是否胜利
    public bool IsPass()
    {
        return true;
    }

}
