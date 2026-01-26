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
    public float BlackImageFadeIn = 0.4f;
    public float BlackImageFadeOut = 1f;
    public HashSet<string> currentLevelCollectedIds = new HashSet<string>();

    public int currentSceneId { get; private set; }
    private SceneMgr()
    {
        sceneInfos = GameDataMgr.Instance.list_LevelData;
        // 监听玩家死亡事件
        GameEvents.PlayerDie += TriggerReload;
        // GameEvents.UpdateRebornPoint += UpdateRebornPoint;
    }
    public void Dispose()
    {
        GameEvents.PlayerDie -= TriggerReload;
        // GameEvents.UpdateRebornPoint -= UpdateRebornPoint;
    }


    // 加载场景数据
    public LevelData GetSceneData(int index)
    {
        return sceneInfos[index];
    }


    // 加载场景并且player复活在指定地点
    public void LoadScene(int index, bool hasSuspend = false)
    {
        currentSceneId = sceneInfos[index].LevelId;
        AsyncOperation ao = SceneManager.LoadSceneAsync(currentSceneId);
        ao.completed += (obj) =>
        {
            // 重生点设置
            var rebornObj = GameObject.Find("RebornPos");
            currentRebornPos = rebornObj != null ? rebornObj.transform.position : Vector2.zero;

            PlayerSaveData data = GameDataMgr.Instance.currentSave;

            Vector2 finalPos;

            // 无中断
            if (!hasSuspend)
            {
                finalPos = currentRebornPos;
            }
            else
            {
                Vector2 playerPos = new Vector2(data.SuspendPosX, data.SuspendPosY);
                finalPos = playerPos;
            }
            InitScene(finalPos);

            currentLevelCollectedIds.Clear();
            if (data.HasSuspendedRecord && data.SuspendLevelId == SceneManager.GetActiveScene().buildIndex)
            {
                foreach (var id in data.SuspendInteractedItems)
                {
                    currentLevelCollectedIds.Add(id);
                }
            }

        };
    }
    // 初始化场景
    public void InitScene(Vector2 playerPos)
    {
        InstiatePlayer(playerPos);
        // UI显示
        UIManager.Instance.ShowPanel<GamePanel>();
        UIManager.Instance.ShowPanel<DeathMask>();
    }
    // 初始化角色信息
    public void InstiatePlayer(Vector2 playerPos)
    {
        GameObject playerPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player/player.prefab");
        if (playerPrefab == null)
        {
            Debug.LogError("Player Prefab not found in Resources folder!");
            return;
        }
        GameObject obj = GameObject.Instantiate(playerPrefab, playerPos, Quaternion.identity);
        playerController = obj.GetComponent<PlayerController>();
    }
    // 重新加载场景
    public async void TriggerReload()
    {
        if (isReloading) return; // 防止连续触发
        isReloading = true;

        await ReloadCurrentLevelAsync();

        isReloading = false;
    }

    private async Task ReloadCurrentLevelAsync()
    {
        // 淡入
        var deathMask = UIManager.Instance.GetPanel<DeathMask>();
        Task fadeTask = Task.CompletedTask;
        if (deathMask != null)
        {
            fadeTask = deathMask.BlackImageFadeIn(BlackImageFadeIn);
        }
        // 加载场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        asyncLoad.allowSceneActivation = false;

        await fadeTask;
        while (asyncLoad.progress < 0.9f)
        {
            await Task.Yield();
        }

        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            await Task.Yield();
        }
        // 初始化
        InitScene(currentRebornPos);
        // 淡出
        deathMask = UIManager.Instance.GetPanel<DeathMask>();
        if (deathMask != null)
        {
            await deathMask.BlackImageFadeOut(BlackImageFadeOut);
        }
    }


    // void UpdateRebornPoint(Vector2 pos)
    // {
    //     currentRebornPos = pos;
    // }

    // 当物体被吃掉时调用
    public void RecordItem(string id)
    {
        if (!currentLevelCollectedIds.Contains(id))
        {
            currentLevelCollectedIds.Add(id);
        }
    }
}
