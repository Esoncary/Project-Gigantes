
using System.Collections.Generic;
using GamePlay.Player.Interface;
using UnityEngine;

public class Switch : LevelItem
{
    [Header("开关行为")]
    [SerializeField] private bool initialIsOn;

    public bool IsOn { get; protected set; }
    protected HashSet<ISwitchable> SwitchableObjects { get; set; }
    private bool _isLeave = true;

    protected override void Start()
    {
        // 1. 初始化基础状态
        IsOn = initialIsOn;

        // 2. 调用基类的 CheckStatus（LevelItem）
        base.Start();

        // 3. 通知关联物体（修复：增加IsOn判断的可读性）
        if (IsOn)
        {
            NotifySwitchableObjects();
        }
    }

    // 重命名Notify为NotifySwitchableObjects，避免语义模糊
    protected void NotifySwitchableObjects()
    {
        if (SwitchableObjects == null || SwitchableObjects.Count == 0) return;

        // 修复：遍历副本避免集合修改异常
        foreach (var obj in new List<ISwitchable>(SwitchableObjects))
        {
            if (obj == null)
            {
                SwitchableObjects.Remove(obj);
                continue;
            }

            if (IsOn) obj.OnSwitchOn();
            else obj.OnSwitchOff();
        }
    }

    // --- 核心修复：重写存档处理逻辑 ---
    protected override void HandleAlreadyInteracted()
    {
        // 如果存档显示已触发，设为开启状态
        IsOn = true;
        Debug.Log($"[{gameObject.name}] 开关已触发（存档），设为开启状态");
    }

    // 修复：仅对Player触发，且排除Key子类
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 排除Key子类（Key有自己的触发逻辑）
        if (GetComponent<GamePlay.Pickups.Key>()) return;

        // 2. 仅响应Player碰撞
        if (!collision.CompareTag("Player")) return;

        // 3. 触发Toggle逻辑
        if (_isLeave)
        {
            _isLeave = false;
            Toggle();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 仅Player退出时重置
        if (collision.CompareTag("Player"))
        {
            _isLeave = true;
        }
    }

    public void RegisterSwitchable(ISwitchable switchable)
    {
        SwitchableObjects ??= new HashSet<ISwitchable>();
        if (!SwitchableObjects.Contains(switchable))
        {
            SwitchableObjects.Add(switchable);
        }
    }

    public void Toggle()
    {
        SetState(!IsOn);
    }

    public void SetState(bool state)
    {
        IsOn = state;

        // 开关激活时记录存档
        if (IsOn)
        {
            OnInteract();
        }

        NotifySwitchableObjects();
    }
}