using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMgr
{
    // 所有存档
    private List<PlayerSaveData> list_PlayerSaveData;

    // 存档的个数
    private int saveDateNum;

    // 当前使用的存档
    public PlayerSaveData currentSave;

    // 当前存档索引
    private int currentSaveIndex;

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
        list_PlayerSaveData = JsonMgr.Instance.LoadData<List<PlayerSaveData>>("PlayerSaveData");
        musicDatas = JsonMgr.Instance.LoadData<MusicData>("MusicData") ?? new MusicData();
        list_RoleData = JsonMgr.Instance.LoadData<List<RoleData>>("RoleData") ?? new List<RoleData>();
        list_LevelData = JsonMgr.Instance.LoadData<List<LevelData>>("LevelData") ?? new List<LevelData>();
        // monsterInfos = JsonMgr.Instance.LoadData<List<MonsterInfo>>("MonsterInfo");

        if (list_PlayerSaveData == null)
        {
            list_PlayerSaveData = new List<PlayerSaveData>();
        }

        if (list_PlayerSaveData.Count < saveDateNum)
        {
            for (int i = list_PlayerSaveData.Count; i < saveDateNum; i++)
            {
                PlayerSaveData playerSaveData = new PlayerSaveData();
                // 初始化关卡数据
                foreach (LevelData item in list_LevelData)
                {
                    playerSaveData.LevelProgress.Add(item.LevelId, new LevelProgressData(item.TotalCollectibles));
                }
                list_PlayerSaveData.Add(playerSaveData);
            }
        }
    }

    // 存储玩家存档数据
    public void SavePlayerSaveData()
    {
        // fix-bug 显示UI加载 并且改成异步

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

}
