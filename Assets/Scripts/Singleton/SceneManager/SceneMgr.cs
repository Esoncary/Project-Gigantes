using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

public class SceneMgr
{
    public List<LevelData> sceneInfos;
    public Vector2 currentRebornPos;
    public PlayerController playerController;
    private bool isReloading = false; // 防止重复触发死亡

    private static SceneMgr instance = new SceneMgr();
    public static SceneMgr Instance => instance;
    private SceneMgr()
    {
        sceneInfos = GameDataMgr.Instance.list_LevelData;
        // 监听玩家死亡事件
        GameEvents.PlayerDie += OnPlayerDie;
        GameEvents.UpdateRebornPoint += UpdateRebornPoint;
    }
    public void Dispose()
    {
        GameEvents.PlayerDie -= OnPlayerDie;
        GameEvents.UpdateRebornPoint -= UpdateRebornPoint;
    }
    // private PlayerObj playerObj;
    public void InitInfo()
    {
        InstiatePlayer();
        // UI显示
        UIManager.Instance.ShowPanel<GamePanel>();
        UIManager.Instance.ShowPanel<DeathMask>();
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

    // 初始化角色信息
    public void InstiatePlayer()
    {
        GameObject playerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/player.prefab");

        var rebornObj = GameObject.Find("RebornPos");
        currentRebornPos = rebornObj != null ? rebornObj.transform.position : Vector2.zero;
        GameObject obj = GameObject.Instantiate(playerPrefab, currentRebornPos, Quaternion.identity);
        playerController = obj.GetComponent<PlayerController>();
    }

    private async void OnPlayerDie()
    {
        if (isReloading) return; // 防止连续触发
        isReloading = true;

        await ReloadCurrentLevelAsync();

        isReloading = false;
    }

    void UpdateRebornPoint(Vector2 pos)
    {
        currentRebornPos = pos;
    }
    private async Task ReloadCurrentLevelAsync()
    {
        Debug.Log("sfasfas");
        // 淡入
        await UIManager.Instance.GetPanel<DeathMask>().BlackImageFadeIn(0.4f);

        await ResetLevelState();
        // 淡出
        await UIManager.Instance.GetPanel<DeathMask>().BlackImageFadeOut(0.4f);
    }
    private async Task ResetLevelState()
    {
        playerController.transform.position = currentRebornPos;
        // 所有机关重置

    }
}
