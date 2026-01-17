using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSaveData //存档
{
    public int MaxUnlockedLevelId;
    public float TotalPlayTime;   // 总游玩时间（秒）
    public Dictionary<int, LevelProgressData> LevelProgress;
    public HashSet<string> CollectedGlobalItems;

    public PlayerSaveData()
    {
        MaxUnlockedLevelId = 1;
        TotalPlayTime = 0;
        LevelProgress = new Dictionary<int, LevelProgressData>();
        CollectedGlobalItems = new HashSet<string>();
    }
}
