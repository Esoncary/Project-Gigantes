using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.IO;
using Unity.VisualScripting;
using System;

public class SceneMgr
{
    // 所有场景的数据信息
    public List<LevelData> sceneInfos => GameDataMgr.Instance?.list_LevelData;

    // 出生点
    public Vector2 currentRebornPos { get; set; }

    // 角色控制器
    public PlayerController playerController { get; private set; }
    public GameObject playerObj { get; private set; }

    // 防止重复触发死亡
    private bool isReloading = false;

    // 场景过渡时间
    public float BlackImageFadeIn = 0.4f;
    public float BlackImageFadeOut = 1f;


    private static SceneMgr instance = new SceneMgr();
    public static SceneMgr Instance => instance;


    private SceneMgr()
    {
        // 监听玩家死亡事件
        GameEvents.PlayerDie += OnPlayerDie;
    }
    public void Dispose()
    {
        GameEvents.PlayerDie -= OnPlayerDie;
    }
    // 监听玩家死亡（重命名更清晰）
    private void OnPlayerDie()
    {
        GameDataMgr.Instance.ClearSessionData();
        TriggerReload();
    }
    // 加载场景数据
    public LevelData GetSceneData(int index) => sceneInfos[index];

    // 初始化场景
    public async Task InitScene(Vector2 playerPos)
    {
        InstantiatePlayer(playerPos);
        // UI显示
        UIManager.Instance.ShowPanel<GamePanel>();
        UIManager.Instance.ShowPanel<DeathMask>();
        UIManager.Instance.GetPanel<GamePanel>().RefreshKeyUI();
        await Task.CompletedTask;
    }

    // 初始化角色信息
    public void InstantiatePlayer(Vector2 playerPos)
    {
        if (playerObj == null)
        {
            playerObj = GameObject.FindWithTag("Player");
        }
        if (playerObj == null)
        {
            GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player/player_hmp");
            if (playerPrefab == null)
            {
                Debug.LogError($"[SceneMgr] 找不到玩家预制体");
                return;
            }
            playerObj = GameObject.Instantiate(playerPrefab, playerPos, Quaternion.identity);
            playerController = playerObj.GetComponent<PlayerController>();

            // 启动初始化保护计时器（确保能量归零）
            playerController.energyResetProtectionTimer = playerController.energyResetProtectionTime;
        }
        else
        {
            playerObj.transform.position = playerPos;
            if (playerController == null) playerController = playerObj.GetComponent<PlayerController>();

            playerController.StateMachine.Initialize(playerController.IdleState);

            // 启动初始化保护计时器（确保能量归零）
            playerController.energyResetProtectionTimer = playerController.energyResetProtectionTime;
        }

        // 绑定新角色实例到相机上
        if (VirtualCameraController.Instance != null && playerObj != null)
        {
            VirtualCameraController.Instance.ResetCameraTarget(playerObj.transform);
        }
    }

    // 场景过度
    public async Task SceneTransitionAsync(System.Func<Task> something = null)
    {
        // 淡入
        UIManager.Instance.ShowPanel<DeathMask>();
        var deathMask = UIManager.Instance.GetPanel<DeathMask>();
        if (deathMask != null)
        {
            if (deathMask.maskImage != null) deathMask.maskImage.fillAmount = 0;
            await deathMask.BlackImageFadeIn(BlackImageFadeIn);
        }
        // 执行事件
        if (something != null)
        {
            await something();
        }
        // 淡出
        deathMask = UIManager.Instance.GetPanel<DeathMask>();
        if (deathMask != null)
        {
            await deathMask.BlackImageFadeOut(BlackImageFadeOut);
        }
    }

    // 加载场景
    // public async void LoadSceneAsync(int sceneId, Action callback = null)
    // {
    //     await SceneTransitionAsync(async () =>
    //     {
    //         GameDataMgr.Instance.SetCurrentLevelId(sceneId);
    //         AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneId);
    //         asyncLoad.allowSceneActivation = false;
    //         while (asyncLoad.progress < 0.9f)
    //         {
    //             await Task.Yield();
    //         }

    //         asyncLoad.allowSceneActivation = true;

    //         while (!asyncLoad.isDone)
    //         {
    //             await Task.Yield();
    //         }
    //         // 执行事件
    //         callback?.Invoke();
    //     });
    // }
    public async void LoadSceneAsync(string sceneName, Action callback = null)
    {
        await SceneTransitionAsync(async () =>
        {
            // 注意：这里不再在 LoadSceneAsync 内部设置 ID，因为调用者应该已经设置好了
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName); // 改为按名称加载
            asyncLoad.allowSceneActivation = false;
            while (asyncLoad.progress < 0.9f)
            {
                await Task.Yield();
            }

            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }
            callback?.Invoke();
        });
    }
    // 重新加载场景
    // public async void TriggerReload()
    // {
    //     if (isReloading) return; // 防止连续触发
    //     isReloading = true;

    //     // await SceneTransitionAsync(() => InitScene(currentRebornPos));
    //     await Task.Delay(500);
    //     LoadSceneAsync(GameDataMgr.Instance.currentLevelId, async () =>
    //     {
    //         // Debug.Log("GameDataMgr.Instance.currentSave.suspendData.hasSuspendedRecord" + GameDataMgr.Instance.currentSave.suspendData.hasSuspendedRecord);
    //         if (!GameDataMgr.Instance.currentSave.suspendData.hasSuspendedRecord)
    //         {
    //             currentRebornPos = GameObject.Find("RebornPos").transform.position;

    //         }
    //         else
    //         {
    //             currentRebornPos = new Vector2(GameDataMgr.Instance.currentSave.suspendData.suspendPosX, GameDataMgr.Instance.currentSave.suspendData.suspendPosY);
    //             GameDataMgr.Instance.currentLevelCollectedIds.Clear();
    //             foreach (var id in GameDataMgr.Instance.currentSave.suspendData.interactedItems)
    //             {
    //                 GameDataMgr.Instance.currentLevelCollectedIds.Add(id);
    //             }
    //             Debug.Log(GameDataMgr.Instance.currentLevelCollectedIds.Count);
    //         }

    //         Debug.Log("位置" + currentRebornPos);
    //         await InitScene(currentRebornPos);
    //     });


    //     isReloading = false;
    // }
    public async void TriggerReload()
    {
        if (isReloading) return;
        isReloading = true;

        await Task.Delay(500);

        // 获取当前关卡的配置信息
        int currentId = GameDataMgr.Instance.currentLevelId;
        string sceneName = sceneInfos.Find(x => x.LevelId == currentId).SceneName;

        LoadSceneAsync(sceneName, async () => // 传入名称
        {
            if (!GameDataMgr.Instance.currentSave.suspendData.hasSuspendedRecord)
            {
                currentRebornPos = GameObject.Find("RebornPos").transform.position;
            }
            else
            {
                currentRebornPos = new Vector2(GameDataMgr.Instance.currentSave.suspendData.suspendPosX, GameDataMgr.Instance.currentSave.suspendData.suspendPosY);
                GameDataMgr.Instance.currentLevelCollectedIds.Clear();
                foreach (var id in GameDataMgr.Instance.currentSave.suspendData.interactedItems)
                {
                    GameDataMgr.Instance.currentLevelCollectedIds.Add(id);
                }
                // ===================== 新增：加载完成后还原机器 =====================
                RestoreMachinesState();
            }
            await InitScene(currentRebornPos);
        });

        isReloading = false;
    }
    // 加载场景并且player复活在指定地点
    // public void LoadGameScene(int sceneId)
    // {
    //     GameDataMgr.Instance.ShowData();
    //     int currentLevelId = sceneInfos[sceneId].LevelId;
    //     GameDataMgr.Instance.SetCurrentLevelId(currentLevelId);
    //     // Debug.Log("123");
    //     AsyncOperation ao = SceneManager.LoadSceneAsync(currentLevelId);
    //     ao.completed += async (obj) =>
    //     {

    //         Vector2 finalPos;

    //         SuspendData suspendData = GameDataMgr.Instance.currentSave.suspendData;
    //         // 无中断
    //         if (!suspendData.hasSuspendedRecord)
    //         {
    //             // 重生点
    //             var rebornObj = GameObject.Find("RebornPos");
    //             finalPos = rebornObj != null ? rebornObj.transform.position : Vector2.zero;

    //             suspendData.interactedItems.Clear();
    //             GameDataMgr.Instance.currentLevelCollectedIds.Clear();
    //         }
    //         else
    //         {
    //             finalPos = new Vector2(suspendData.suspendPosX, suspendData.suspendPosY);

    //             GameDataMgr.Instance.currentLevelCollectedIds.Clear();
    //             foreach (var id in suspendData.interactedItems)
    //             {
    //                 GameDataMgr.Instance.currentLevelCollectedIds.Add(id);
    //             }
    //         }
    //         GameDataMgr.Instance.currentSave.suspendData = suspendData;
    //         currentRebornPos = finalPos;
    //         await InitScene(finalPos);
    //     };
    // }
    public void LoadGameScene(int sceneId)
    {
        GameDataMgr.Instance.ShowData();
        LevelData data = sceneInfos[sceneId];
        Debug.Log("data.LevelId" + data.LevelId);
        Debug.Log("data.SceneName" + data.SceneName);
        GameDataMgr.Instance.SetCurrentLevelId(data.LevelId);
        // Debug.Log("123");
        AsyncOperation ao = SceneManager.LoadSceneAsync(data.SceneName);
        ao.completed += async (obj) =>
        {

            Vector2 finalPos;

            SuspendData suspendData = GameDataMgr.Instance.currentSave.suspendData;
            // 无中断
            if (!suspendData.hasSuspendedRecord)
            {
                // 重生点
                var rebornObj = GameObject.Find("RebornPos");
                finalPos = rebornObj != null ? rebornObj.transform.position : Vector2.zero;

                suspendData.interactedItems.Clear();
                GameDataMgr.Instance.currentLevelCollectedIds.Clear();
            }
            else
            {
                finalPos = new Vector2(suspendData.suspendPosX, suspendData.suspendPosY);

                GameDataMgr.Instance.currentLevelCollectedIds.Clear();
                foreach (var id in suspendData.interactedItems)
                {
                    GameDataMgr.Instance.currentLevelCollectedIds.Add(id);
                }
                // ===================== 新增：加载完成后还原机器 =====================
                RestoreMachinesState();
            }
            GameDataMgr.Instance.currentSave.suspendData = suspendData;
            currentRebornPos = finalPos;
            await InitScene(finalPos);
        };
    }

    // 根据场景名得到场景的id
    public int GetSceneIdByName(string sceneName)
    {
        int count = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = Path.GetFileNameWithoutExtension(path);

            if (name == sceneName)
                return i;
        }

        return -1;
    }

    public void UpdateCheckpoint(Vector2 pos)
    {
        // 1. 更新当前内存中的重生位置
        currentRebornPos = pos;

        // 2. 将数据同步到存档数据中
        var suspendData = GameDataMgr.Instance.currentSave.suspendData;
        suspendData.suspendPosX = pos.x;
        suspendData.suspendPosY = pos.y;
        suspendData.suspendLevelId = GameDataMgr.Instance.currentLevelId;
        suspendData.hasSuspendedRecord = true; // 标记现在有存档记录了
        // ===================== 新增：保存所有可存档机器的状态 =====================
        suspendData.savedMachines.Clear();
        GameObject[] allMachines = GameObject.FindGameObjectsWithTag("SaveableMachine");
        foreach (GameObject machine in allMachines)
        {
            // 获取唯一标识组件
            MachineIdentity identity = machine.GetComponent<MachineIdentity>();
            if (identity == null)
            {
                Debug.LogError($"机器 {machine.name} 缺少 MachineIdentity 组件，无法保存！");
                continue;
            }
            // 构建存档数据并加入列表
            MachineSaveData data = new MachineSaveData(identity);
            suspendData.savedMachines.Add(data);
        }


        GameDataMgr.Instance.currentSave.suspendData = suspendData;
        // Debug.Log("存档数据" + GameDataMgr.Instance.currentSave.suspendData.hasSuspendedRecord);
        foreach (var id in GameDataMgr.Instance.currentLevelCollectedIds)
        {
            if (!suspendData.interactedItems.Contains(id))
            {
                suspendData.interactedItems.Add(id);
            }
        }

        // 4. 立即保存到本地文件
        GameDataMgr.Instance.SavePlayerSaveData();
    }
    /// <summary>
    /// 根据存档数据还原场景内所有机器的状态
    /// </summary>

    private void RestoreMachinesState()
    {
        var suspendData = GameDataMgr.Instance.currentSave.suspendData;
        if (!suspendData.hasSuspendedRecord || suspendData.savedMachines.Count == 0)
            return;

        // 1. 先还原所有根物体（父物体）
        foreach (var machineData in suspendData.savedMachines)
        {
            if (string.IsNullOrEmpty(machineData.parentMachineId))
            {
                RestoreSingleMachine(machineData, isRoot: true);
            }
        }

        // 2. 再还原所有子物体
        foreach (var machineData in suspendData.savedMachines)
        {
            if (!string.IsNullOrEmpty(machineData.parentMachineId))
            {
                RestoreSingleMachine(machineData, isRoot: false);
            }
        }
    }

    // 单独还原一个物体的辅助方法
    private void RestoreSingleMachine(MachineSaveData data, bool isRoot)
    {
        GameObject[] allMachines = GameObject.FindGameObjectsWithTag("SaveableMachine");
        foreach (GameObject machine in allMachines)
        {
            MachineIdentity identity = machine.GetComponent<MachineIdentity>();
            if (identity != null && identity.machineUniqueId == data.machineId)
            {
                Transform transform = machine.transform;
                if (isRoot)
                {
                    // 根物体用世界坐标还原
                    transform.position = new Vector3(data.worldPosX, data.worldPosY, data.worldPosZ);
                    transform.rotation = Quaternion.Euler(data.worldRotX, data.worldRotY, data.worldRotZ);
                }
                else
                {
                    // 子物体用局部坐标还原
                    transform.localPosition = new Vector3(data.localPosX, data.localPosY, data.localPosZ);
                    transform.localRotation = Quaternion.Euler(data.localRotX, data.localRotY, data.localRotZ);
                }
                break;
            }
        }
    }
}
