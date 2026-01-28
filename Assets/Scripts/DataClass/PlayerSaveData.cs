using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSaveData //存档
{

    public int maxUnlockedLevelId; // 最大的解锁的关卡ID
    public float totalPlayTime;   // 总游玩时间（秒）
    public List<LevelProgressData> listlevelProgress; // 关卡进程
    // 中断信息
    public SuspendData suspendData;
    public PlayerSaveData()
    {
        maxUnlockedLevelId = 0;
        totalPlayTime = 0f;
        listlevelProgress = new List<LevelProgressData>();
        suspendData = new SuspendData();
    }
}
