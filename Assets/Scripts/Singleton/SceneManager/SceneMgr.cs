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
    public List<LevelData> sceneInfos;

    // 出生点
    public Vector2 currentRebornPos;

    // 角色控制器
    public PlayerController playerController;

    // 防止重复触发死亡
    private bool isReloading = false;

    // 场景过渡时间
    public float BlackImageFadeIn = 0.4f;
    public float BlackImageFadeOut = 1f;

    private static SceneMgr instance = new SceneMgr();
    public static SceneMgr Instance => instance;


    private SceneMgr()
    {
        if (GameDataMgr.Instance != null)
        {
            sceneInfos = GameDataMgr.Instance.list_LevelData;
        }
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
        TriggerReload();
    }
    // 加载场景数据
    public LevelData GetSceneData(int index)
    {
        return sceneInfos[index];
    }

    // 初始化场景
    public void InitScene(Vector2 playerPos)
    {
        InstantiatePlayer(playerPos);
        // UI显示
        UIManager.Instance.ShowPanel<GamePanel>();
        UIManager.Instance.ShowPanel<DeathMask>();
    }

    // 初始化角色信息
    public void InstantiatePlayer(Vector2 playerPos)
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
    public async void LoadSceneAsync(int sceneId, Action callback = null)
    {
        await SceneTransitionAsync(async () =>
        {
            GameDataMgr.Instance.SetCurrentLevelId(sceneId);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneId);
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
            // 执行事件
            callback?.Invoke();
        });
    }

    // 重新加载场景
    public async void TriggerReload()
    {
        if (isReloading) return; // 防止连续触发
        isReloading = true;

        LoadSceneAsync(GameDataMgr.Instance.currentLevelId, () =>
        {
            InitScene(currentRebornPos);
        });


        isReloading = false;
    }

    // 加载场景并且player复活在指定地点
    public void LoadGameScene(int sceneId)
    {
        int currentLevelId = sceneInfos[sceneId].LevelId;
        GameDataMgr.Instance.SetCurrentLevelId(currentLevelId);
        Debug.Log("123");
        AsyncOperation ao = SceneManager.LoadSceneAsync(currentLevelId);
        ao.completed += (obj) =>
        {
            // 重生点
            var rebornObj = GameObject.Find("RebornPos");
            currentRebornPos = rebornObj != null ? rebornObj.transform.position : Vector2.zero;
            Vector2 finalPos;

            SuspendData suspendData = GameDataMgr.Instance.currentSave.suspendData;
            // 无中断
            if (!suspendData.hasSuspendedRecord)
            {
                finalPos = currentRebornPos;

                suspendData.interactedItems.Clear();
                GameDataMgr.Instance.currentLevelCollectedIds.Clear();
            }
            else
            {
                Vector2 playerPos = new Vector2(suspendData.suspendPosX, suspendData.suspendPosY);
                finalPos = playerPos;

                GameDataMgr.Instance.currentLevelCollectedIds.Clear();
                foreach (var id in suspendData.interactedItems)
                {
                    GameDataMgr.Instance.currentLevelCollectedIds.Add(id);
                }
            }
            InitScene(finalPos);
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



}
