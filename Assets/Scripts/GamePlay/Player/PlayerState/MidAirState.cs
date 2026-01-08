using UnityEngine;

public class MidAirState : PlayerState
{
    public MidAirState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    public override void Enter()
    {
        base.Enter();
        // 进入空中时，不需要重置重力，因为我们需要重力自然作用
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // --- 1. 精致化：根据垂直速度切换上升/下落动画 ---
        if (player.rb.velocity.y > 0.1f)
        {
            player.PlayAnimation("Jump_Up");
        }
        else if (player.rb.velocity.y < -0.1f)
        {
            player.PlayAnimation("Fall_Down");
        }

        // --- 2. 状态切换：落地 ---
        if (player.groundedCheckerManager.isGrounded && player.rb.velocity.y <= 0.01f)
        {
            // 如果落地时还有较大的水平输入，直接进入 Run，否则进入 Brake 或 Idle
            if (Mathf.Abs(player.InputX) > 0.01f)
                stateMachine.ChangeState(player.RunState);
            else
                stateMachine.ChangeState(player.BrakeState);

            return;
        }

        // --- 3. 状态切换：攀爬 ---
        // 监测左右手检测器，如果贴墙且按下攀爬键
        if (player.canClimb && Input.GetKeyDown(KeyCode.J))
        {
            stateMachine.ChangeState(player.ClimbState);
            return;
        }

        // --- 4. 状态切换：向量释放 ---
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            stateMachine.ChangeState(player.ReleaseState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // --- 5. 水平空中移动控制 ---
        HandleAirMovement();

        // --- 6. 持续跳跃加力 (长按跳得高) ---
        HandleVariableJump();

        // --- 7. 到达顶峰时的重力缩减 (Apex Float) ---
        HandleApexFloat();
    }

    private void HandleAirMovement()
    {
        float inputX = player.InputX;

        // 只有在未达上限，或者正在反向转向时，才允许加力
        if (Mathf.Abs(player.rb.velocity.x) < player.maxMoveSpeedInMidAir || Mathf.Sign(inputX) != Mathf.Sign(player.rb.velocity.x))
        {
            player.rb.AddForce(new Vector2(inputX * player.moveSpeedAccInMidAir, 0));
        }

        // 空中强制减速：如果弹射后的速度依然超过上限，且已经过了保护期
        if (Mathf.Abs(player.rb.velocity.x) > player.maxMoveSpeedInMidAir && player.inputLockTimer <= 0)
        {
            // 施加空气阻力
            float decelerateForce = -Mathf.Sign(player.rb.velocity.x) * player.enforceMoveAccSpeedInMidAir;
            player.rb.AddForce(new Vector2(decelerateForce, 0));
        }
    }

    private void HandleVariableJump()
    {
        // 这里的逻辑对应你之前的 varJumpTimer
        if (Input.GetKey(KeyCode.Space) && player.varJumpTimer > 0)
        {
            if (player.rb.velocity.y < player.maxJumpSpeed)
            {
                player.rb.AddForce(new Vector2(0, player.jumpSpeedAcc));
            }
        }
    }

    private void HandleApexFloat()
    {
        // 当垂直速度接近 0（到达抛物线顶端）时，减小重力产生滞空感
        if (Mathf.Abs(player.rb.velocity.y) < player.gravityContractionThreshold)
        {
            player.rb.gravityScale = player.gravityContractionScale;
        }
        else
        {
            player.rb.gravityScale = 1.0f; // 恢复正常重力
        }
    }

    public override void Exit()
    {
        base.Exit();
        // 离开空中状态时，务必恢复重力常数，防止影响其他状态
        player.rb.gravityScale = 1.0f;
    }
}