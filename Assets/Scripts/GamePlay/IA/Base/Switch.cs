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

        // 2. 调用基类的 CheckStatus，它会根据存档决定是否执行 HandleAlreadyInteracted
        base.Start();

        // 3. 如果初始就是开启的，或者被 CheckStatus 设为开启的，通知关联物体
        if (IsOn)
        {
            Notify();
        }
    }

    // --- 核心合并点：重写存档处理逻辑 ---
    protected override void HandleAlreadyInteracted()
    {
        // 如果存档显示这个开关已经触发过了
        // 我们不销毁物体，而是直接把它设为开启状态
        IsOn = true;
        // 注意：这里不需要手动调 Notify，Start 里的逻辑会统一处理
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 排除 Key 类型的特殊处理逻辑（根据你原代码保留）
        if (_isLeave && !GetComponent<GamePlay.Pickups.Key>())
        {
            _isLeave = false;
            Toggle();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isLeave = true;
    }

    public void RegisterSwitchable(ISwitchable switchable)
    {
        SwitchableObjects ??= new HashSet<ISwitchable>();
        SwitchableObjects.Add(switchable);
    }

    public void Toggle()
    {
        SetState(!IsOn);
    }

    public void SetState(bool state)
    {
        IsOn = state;

        // 如果开关被激活了
        if (IsOn)
        {
            OnInteract(); // 调用 LevelItem 的记录逻辑
        }

        Notify();
    }

    protected void Notify()
    {
        if (SwitchableObjects == null) return;

        foreach (var obj in new List<ISwitchable>(SwitchableObjects))
        {
            if (obj == null) continue;

            if (IsOn) obj.OnSwitchOn();
            else obj.OnSwitchOff();
        }
    }
}