using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelProgressData
{
    public bool IsCompleted;        // 是否通关
    public float BestClearTime;      // 最快通关时间
    public float LastClearTime;      // 最近一次通关时间
    public int CollectedCount;       // 本关收集物数量
    public int TotalCollectibles;    // 本关可收集总数
    public bool IsPerfectClear;      // 是否全收集
    public LevelProgressData(int num)
    {
        IsCompleted = false;
        CollectedCount = 0;
        TotalCollectibles = num;
        IsPerfectClear = false;
    }
}
