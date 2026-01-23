using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostReleaseState : PlayerState
{
    //这是一个存续时间极短的状态，其目的在于使角色的release更加纯粹，不受其它物理因素强迫。在这个状态中，我们会在计时器运行的时候削弱冰逐渐恢复玩家的移动加速度，削弱玩家所受的重力影响，从而让角色在高速弹射后有一个短暂的“失控”时间，以便玩家更好地感受到release的威力。

    public PostReleaseState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    public override void Enter()
    {
        base.Enter();
        // 进入时可以播放：喷气特效、拉伸动画、或者让角色变色
    }

    public override void HandleInput()
    {
        //在launched的状态中，屏蔽所有玩家的输入
    }

    public override void LogicUpdate()
    {
        
    }

    public override void PhysicsUpdate()
    {
        //目前来讲，该状态下没有任何物理输出需求
    }

    public override void Exit()
    {
        base.Exit();
        
    }
}
