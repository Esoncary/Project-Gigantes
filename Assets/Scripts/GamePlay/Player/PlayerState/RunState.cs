using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 1. 状态切换：松手 -> 刹车
        if (Mathf.Abs(player.InputX) < 0.01f)
        {
            stateMachine.ChangeState(player.BrakeState);
            return;
        }

        // 2. 状态切换：跳跃
        if (player.jumpBufferTimer > 0 && player.canJump)
        {
            player.InitialJump();
            player.canJump = false; // 落地前只能跳一次
            stateMachine.ChangeState(player.MidAirState);
            return;
        }

        // 3. 状态切换：掉落
        if (!player.groundedCheckerManager.isGrounded)
        {
            stateMachine.ChangeState(player.MidAirState);
            return;
        }

        // 4. 状态切换：释放 (由外部输入触发，但切入 ReleaseState)
        // 注意：这里只需判断是否要进入释放状态，具体的数值由 KineticDevice 处理
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            stateMachine.ChangeState(player.ReleaseState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        float inputX = player.InputX;

        // 转向处理
        if (Mathf.Abs(player.rb.velocity.x) > 0.1f && Mathf.Sign(player.rb.velocity.x) != inputX)
        {
            player.ApplyBrakeForce(player.brakeDeceleraion);
        }

        // 加速与限速逻辑,oberSpeed状态之后的限速还没写
        if (Mathf.Abs(player.rb.velocity.x) < player.minMoveSpeed)
        {
            player.rb.velocity = new Vector2(inputX * player.minMoveSpeed, player.rb.velocity.y);
        }

        if (Mathf.Abs(player.rb.velocity.x) < player.maxMoveSpeed)
        {
            player.rb.AddForce(new Vector2(inputX * player.moveSpeedAcc, 0));
            
        }
        else
        {
            player.rb.velocity = new Vector2(Mathf.Sign(player.rb.velocity.x) * player.maxMoveSpeed, player.rb.velocity.y);
        }
    }
}