using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMgr
{
    // 所有存档
    private List<PlayerSaveData> list_PlayerSaveData;

    // 存档的个数
    private int saveDateNum = 1;

    // 当前使用的存档
    public PlayerSaveData currentSave;

    // 当前存档索引
    private int currentSaveIndex = 0;

    // 关卡配置
    public List<LevelData> list_LevelData { get; private set; }

    //角色数据
    public List<RoleData> list_RoleData;

    //音乐数据 
    public MusicData musicDatas;

    //怪物数据
    // public List<MonsterInfo> monsterInfos;


    private static GameDataMgr instance = new GameDataMgr();
    public static GameDataMgr Instance => instance;

    public GameDataMgr()
    {
        list_PlayerSaveData = JsonMgr.Instance.LoadData<List<PlayerSaveData>>("PlayerSaveData") ?? new List<PlayerSaveData>();

        if (list_PlayerSaveData.Count == 0)
        {
            list_PlayerSaveData.Add(new PlayerSaveData());
        }

        musicDatas = JsonMgr.Instance.LoadData<MusicData>("MusicData") ?? new MusicData();
        list_RoleData = JsonMgr.Instance.LoadData<List<RoleData>>("RoleData") ?? new List<RoleData>();
        list_LevelData = JsonMgr.Instance.LoadData<List<LevelData>>("LevelData") ?? new List<LevelData>();

        // if (list_PlayerSaveData.Count < saveDateNum)
        // {
        //     for (int i = list_PlayerSaveData.Count; i < saveDateNum; i++)
        //     {
        //         PlayerSaveData playerSaveData = new PlayerSaveData();
        //         // 初始化关卡数据
        //         foreach (LevelData item in list_LevelData)
        //         {
        //             playerSaveData.LevelProgress.Add(item.LevelId, new LevelProgressData(item.TotalCollectibles));
        //         }
        //         list_PlayerSaveData.Add(playerSaveData);
        //     }
        // }

        GameEvents.PlayerDie += ClearSuspendData;
    }

    // 存储玩家存档数据
    public void SavePlayerSaveData()
    {
        if (currentSaveIndex < 0 || currentSaveIndex >= list_PlayerSaveData.Count)
        {
            Debug.LogError("当前存档索引非法，无法保存");
            return;
        }
        list_PlayerSaveData[currentSaveIndex] = currentSave;
        JsonMgr.Instance.SaveData(list_PlayerSaveData, "PlayerSaveData");
    }


    // 加载选择的存档
    public void LoadChoosePlayerSaveData(int index)
    {
        if (index < 0 || index > saveDateNum)
        {
            Debug.LogError("存档索引不正确");
            return;
        }
        currentSaveIndex = index;
        currentSave = list_PlayerSaveData[currentSaveIndex];
    }

    // 存储音乐数据
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicDatas, "MusicData");
    }

    // 通关保存
    public void SaveLevelComplete(int levelId, float timeCost, int collectedNum)
    {
        PlayerSaveData data = currentSave;

        // TotalPlayTime

        if (levelId >= data.MaxUnlockedLevelId)
        {
            data.MaxUnlockedLevelId = levelId + 1;
        }

        // 更新本关卡的数据
        LevelProgressData levelProgressData;

        // 检查以前是否玩过这一关
        while (data.ListlevelProgress.Count < levelId)
        {
            LevelProgressData newData = new LevelProgressData();
            data.ListlevelProgress.Add(newData);
        }
        levelProgressData = data.ListlevelProgress[levelId];

        // 是否通关
        levelProgressData.IsCompleted = true;

        // 最快通关时间
        if (levelProgressData.BestClearTime <= 0.001f || timeCost < levelProgressData.BestClearTime)
        {
            levelProgressData.BestClearTime = timeCost;
        }
        // 最大收集数量
        if (collectedNum > levelProgressData.CollectedCount)
        {
            levelProgressData.CollectedCount = collectedNum;
        }
        // 是否全收集
        if (levelProgressData.CollectedCount >= list_LevelData[levelId].TotalCollectibles)
        {
            levelProgressData.IsPerfectClear = true;
        }

        // 清除“中途退出”的记录
        data.HasSuspendedRecord = false;
        data.SuspendLevelId = -1; // 重置为一个无效ID
        data.SuspendPosX = 0;
        data.SuspendPosY = 0;
        data.SuspendInteractedItems.Clear();

        currentSave = data;
        SavePlayerSaveData();

        Debug.Log($"关卡 {levelId} 结算完成！已保存。解锁进度: {data.MaxUnlockedLevelId}");
    }

    // 保存中断存档
    public void SaveSuspendData(int levelId, Vector2 playerPos)
    {
        currentSave.HasSuspendedRecord = true;
        currentSave.SuspendLevelId = levelId;
        currentSave.SuspendPosX = playerPos.x;
        currentSave.SuspendPosY = playerPos.y;
        currentSave.SuspendInteractedItems.Clear();
        foreach (var id in SceneMgr.Instance.currentLevelCollectedIds)
        {
            currentSave.SuspendInteractedItems.Add(id);
        }

        SavePlayerSaveData();
        Debug.Log($"中断存档已保存：关卡{levelId} 位置{playerPos}");
    }
    // 清除中断存档
    public void ClearSuspendData()
    {
        currentSave.HasSuspendedRecord = false;
        currentSave.SuspendLevelId = -1;
        currentSave.SuspendPosX = 0;
        currentSave.SuspendPosY = 0;
        currentSave.SuspendInteractedItems.Clear();
        SavePlayerSaveData();
    }
}
