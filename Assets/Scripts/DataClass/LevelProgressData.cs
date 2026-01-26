using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelProgressData
{
    public bool IsCompleted;        // 是否通关
    public float BestClearTime;      // 最快通关时间
    public int CollectedCount;       // 已收集到的数量
    public bool IsPerfectClear;      // 是否全收集
    public LevelProgressData()
    {
        IsCompleted = false;
        BestClearTime = 0;
        CollectedCount = 0;
        IsPerfectClear = false;
    }
}
