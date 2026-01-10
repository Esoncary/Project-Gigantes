using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    public override void Enter()
    {
        base.Enter(); // 自动播动画
        player.rb.velocity = new Vector2(0, player.rb.velocity.y); // 进入静止时可以选清空X速度
    }

    public override void HandleInput()
    {
        base.HandleInput();
        // 可以在这里处理输入缓冲
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 1. 如果有水平输入，切到跑步
        if (Mathf.Abs(player.InputX) > 0.01f)
        {
            stateMachine.ChangeState(player.RunState);
        }

        // 2. 如果按下跳跃，直接切到空中(或跳跃)状态
        if (player.jumpBufferTimer > 0 && player.canJump)
        {
            player.InitialJump();
            player.canJump = false; // 落地前只能跳一次
            stateMachine.ChangeState(player.MidAirState);
            return;
        }

        // 3. 如果突然脚下一空(掉下去了)，切到空中
        if (!player.groundedCheckerManager.isGrounded)
        {
            stateMachine.ChangeState(player.MidAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // 在这里执行刹车，确保角色站得稳
        if (Mathf.Abs(player.RB.velocity.x) > 0.01f)
        {
            player.Brake();
        }
    }
}