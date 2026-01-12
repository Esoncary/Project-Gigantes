using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchedState : PlayerState
{


    public LaunchedState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        // 1. 计时器判定：如果时间到了，恢复控制权
        if (player.speedLimitOffTimer <= 0)
        {
            player.speedLimitOffTimer = -1; //关闭speedLimitOff计时器
            stateMachine.ChangeState(player.MidAirState);
            return;
        }

        //// 2. 碰撞判定：如果高速弹射中撞到了墙，通常应该提前结束锁定
        //if (player.isGrounded )
        //{
            
        //    stateMachine.ChangeState(player.RunState);
        //    return;
        //}
        //if (player.canClimb)
        //{
        //    stateMachine.ChangeState(player.MidAirState);
        //    return;
        //}
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
