using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSaveData //存档
{
    public int MaxUnlockedLevelId;
    public float TotalPlayTime;   // 总游玩时间（秒）
    public List<LevelProgressData> ListlevelProgress;
    public List<string> SuspendInteractedItems; // 记录已收集的道具ID
    public bool HasSuspendedRecord; // 是否有中断记录
    public int SuspendLevelId;      // 中断时的关卡ID
    public float SuspendPosX;       // 玩家位置 X
    public float SuspendPosY;       // 玩家位置 Y
    public PlayerSaveData()
    {
        MaxUnlockedLevelId = 0;
        TotalPlayTime = 0f;
        ListlevelProgress = new List<LevelProgressData>();
        SuspendInteractedItems = new List<string>();
        HasSuspendedRecord = false;
        SuspendLevelId = -1;
        SuspendPosX = 0f;
        SuspendPosY = 0f;
    }
    public override string ToString()
    {
        return $"MaxLevel: {MaxUnlockedLevelId}, Time: {TotalPlayTime},";
    }
}
