using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SuspendData // 中断信息
{
    public bool hasSuspendedRecord; // 是否有中断记录
    public List<string> interactedItems; // 记录已收集的道具ID
    public int suspendLevelId;      // 中断时的关卡ID
    public float suspendPosX;       // 玩家位置 X
    public float suspendPosY;       // 玩家位置 Y
    public List<MachineSaveData> savedMachines;
    public SuspendData()
    {
        hasSuspendedRecord = false;
        interactedItems = new List<string>();
        suspendLevelId = -1;
        suspendPosX = 0f;
        suspendPosY = 0f;
        // ===================== 新增：机器存档列表 =====================
        savedMachines = new List<MachineSaveData>();
    }
    public void ToString()
    {
        Debug.Log("hasSuspendedRecord" + hasSuspendedRecord);
    }
}
