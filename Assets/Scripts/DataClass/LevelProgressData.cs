using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelProgressData
{
    public bool isCompleted;        // 是否通关
    public float bestClearTime;      // 最快通关时间
    public int collectedCount;       // 已收集到的数量
    public bool isPerfectClear;      // 是否全收集
    public LevelProgressData()
    {
        isCompleted = false;
        bestClearTime = 0;
        collectedCount = 0;
        isPerfectClear = false;
    }
}
