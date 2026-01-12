using UnityEngine;
using static PlayerController;

public class PlayerStateMachine
{
    // 只读属性：外界可以看当前是什么状态，但不能直接通过赋值来修改它
    public PlayerState CurrentState { get; private set; }

    // 初始化：在游戏开始时调用，设定第一个状态（通常是 Idle）
    public void Initialize(PlayerState _initialState)
    {
        CurrentState = _initialState;
        CurrentState.Enter();
    }

    // 状态切换的核心：这是最常用的函数
    public void ChangeState(PlayerState _newState)
    {
        // 1. 先让旧状态收拾东西走人（比如关掉重力、停止某个协程）
        CurrentState.Exit();

        // 2. 换成新状态
        CurrentState = _newState;

        // 3. 让新状态入职（比如播放新动画、给一个初始冲量、开启计时器）
        CurrentState.Enter();
    }
}