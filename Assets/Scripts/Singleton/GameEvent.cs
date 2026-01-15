using System;
using UnityEngine;
public static class GameEvents
{
    // 定义死亡广播频道
    public static Action PlayerDie;
    public static Action<Vector2> UpdateRebornPoint;

    // 呼叫死亡广播的方法
    public static void BroadcastPlayerDieEvent()//播放角色死亡事件
    {
        // 如果有人在收听(LevelManager)，就通知他们
        PlayerDie?.Invoke();
    }

    public static void BroadcastUpdateRebornPoint(Vector2 currentRebornPos)//播放更新重生点事件
    {
        UpdateRebornPoint?.Invoke(currentRebornPos);
    }
}