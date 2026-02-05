using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameDataMgr
{
    // 所有存档
    private List<PlayerSaveData> list_PlayerSaveData;

    // 存档的个数
    private int saveDateNum = 1;

    // 当前使用的存档
    public PlayerSaveData currentSave { get; private set; }

    // 当前存档索引
    private int currentSaveIndex = 0;

    // 关卡配置
    public List<LevelData> list_LevelData { get; private set; }

    //角色数据
    // public List<RoleData> list_RoleData;

    //音乐数据 
    public MusicData musicDatas;

    // 当前关卡id
    public int currentLevelId { get; private set; }

    //怪物数据
    // public List<MonsterInfo> monsterInfos;

    // 关卡中已经收集的物品
    public HashSet<string> currentLevelCollectedIds = new HashSet<string>();
    public int selectedLevelIndex;
    // 单例
    private static GameDataMgr instance = new GameDataMgr();
    public static GameDataMgr Instance => instance;

    public GameDataMgr()
    {
        // 初始化游戏数据
        list_PlayerSaveData = JsonMgr.Instance.LoadData<List<PlayerSaveData>>("PlayerSaveData") ?? new List<PlayerSaveData>();
        // Debug.Log(list_PlayerSaveData.Count);
        if (list_PlayerSaveData.Count == 0)
        {
            list_PlayerSaveData.Add(new PlayerSaveData());
            Debug.Log(list_PlayerSaveData.Count);
            Debug.Log(list_PlayerSaveData[0].suspendData);
            // Debug.Log("list_PlayerSaveData.Count :" + list_PlayerSaveData.Count);
        }

        musicDatas = JsonMgr.Instance.LoadData<MusicData>("MusicData") ?? new MusicData();
        // list_RoleData = JsonMgr.Instance.LoadData<List<RoleData>>("RoleData") ?? new List<RoleData>();
        list_LevelData = JsonMgr.Instance.LoadData<List<LevelData>>("LevelData") ?? new List<LevelData>();

        // 死亡事件
        // GameEvents.PlayerDie += ClearSuspendData;
    }

    // 存储音乐数据
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicDatas, "MusicData");
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
        // Debug.Log(currentSave.suspendData);
    }

    // 保存中断存档
    public void SaveSuspendData(Vector2 playerPos)
    {
        SuspendData suspendData = currentSave.suspendData;

        suspendData.hasSuspendedRecord = true;
        suspendData.suspendLevelId = currentLevelId;
        suspendData.suspendPosX = playerPos.x;
        suspendData.suspendPosY = playerPos.y;
        suspendData.interactedItems.Clear();
        foreach (var id in currentLevelCollectedIds)
        {
            suspendData.interactedItems.Add(id);
        }
        SavePlayerSaveData();
        // Debug.Log($"中断存档已保存：关卡{currentLevelId} 位置{playerPos}");
    }

    // 清除中断存档
    public void ClearSuspendData()
    {
        SuspendData suspendData = currentSave.suspendData;
        suspendData.hasSuspendedRecord = false;
        suspendData.suspendLevelId = -1;
        suspendData.suspendPosX = 0;
        suspendData.suspendPosY = 0;
        suspendData.interactedItems.Clear();
        // ===================== 新增：清空机器数据 =====================
        suspendData.savedMachines.Clear();
        currentSave.suspendData = suspendData;
        ClearSessionData();
        SavePlayerSaveData();
    }

    // 通关保存
    public void SaveLevelComplete(float timeCost, int collectedNum)
    {

        PlayerSaveData data = currentSave;

        // TotalPlayTime todo

        // 解锁下一关
        data.maxUnlockedLevelId = Math.Max(data.maxUnlockedLevelId, currentLevelId + 1);

        // 更新本关卡的数据
        List<LevelProgressData> listlevelProgress = data.listlevelProgress;

        // 检查以前是否玩过这一关
        while (listlevelProgress.Count <= currentLevelId)
        {
            LevelProgressData newData = new LevelProgressData();
            listlevelProgress.Add(newData);
        }
        LevelProgressData levelProgressData = listlevelProgress[currentLevelId];

        // 是否通关
        levelProgressData.isCompleted = true;
        // 最快通关时间
        if (levelProgressData.bestClearTime <= 0.001f || timeCost < levelProgressData.bestClearTime)
        {
            levelProgressData.bestClearTime = timeCost;
        }
        // 最大收集数量
        if (collectedNum > levelProgressData.collectedCount)
        {
            levelProgressData.collectedCount = collectedNum;
        }
        // 是否全收集
        if (levelProgressData.collectedCount >= list_LevelData[currentLevelId].TotalCollectibles)
        {
            levelProgressData.isPerfectClear = true;
        }
        currentSave = data;
        // 清除“中途退出”的记录
        ClearSuspendData();

        Debug.Log($"关卡 {currentLevelId} 结算完成！已保存。解锁进度: {data.maxUnlockedLevelId}");
    }

    // 设置当前关卡id
    public void SetCurrentLevelId(int id)
    {
        // Debug.Log("levelid:" + id);
        currentLevelId = id;
    }

    // 记录当前场景收集物品的id
    public void RecordItem(string id)
    {
        if (!currentLevelCollectedIds.Contains(id))
        {
            currentLevelCollectedIds.Add(id);
            // Debug.Log(id);
        }
    }
    // 当玩家死亡重新加载（未到达检查点）时，清空临时列表
    public void ClearSessionData()
    {
        currentLevelCollectedIds.Clear();
    }
    public void ShowData()
    {
        string str = "已收集物品：";
        foreach (string id in currentLevelCollectedIds)
        {
            str += id + "\n";
        }
        // Debug.Log(str);
    }
}
