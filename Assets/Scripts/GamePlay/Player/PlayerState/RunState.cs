using UnityEngine;

public class RunState : PlayerState
{

    private float stepTimer;
    private float stepInterval = 0.4f; // 调整这个数值来匹配你动画的脚步节奏
    public RunState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }


    public override void Enter()
    {
        base.Enter();
        // 播跑步特效
        player.playerVFXController.PlayRunDust();
        // 刚开始跑时，计时器设为0，确保第一步立即发出声音
        stepTimer = 0;
    }

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
            player.canJump = false; //落地前只能跳一次
            stateMachine.ChangeState(player.MidAirState);
            return;
        }

        // 3. 状态切换：掉落
        if (!player.groundedCheckerManager.isGrounded)
        {
            player.jumpCoyoteTimer = player.jumpCoyoteTime;//开启跳跃土狼时间计时器
            stateMachine.ChangeState(player.MidAirState);
            return;
        }

        // 4. 状态切换：释放 (由外部输入触发，但切入 ReleaseState)
        // 注意：这里只需判断是否要进入释放状态，具体的数值由 KineticDevice 处理
        if (player.CanTriggerRelease())
        {
            player.ConsumeReleaseBuffer();
            stateMachine.ChangeState(player.ReleaseState);
            return;
        }
        if (player.groundedCheckerManager.isGrounded) // 只有在地面时才处理
        {
            HandleFootstepSound();
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        RunMovement();

    }

    void RunMovement()
    {
        // 转向处理
        if (Mathf.Abs(player.rb.velocity.x) > 0.1f && Mathf.Sign(player.rb.velocity.x) != player.InputX)
        {
            player.TurnRoundBrake(player.turnRoundBrakeDec);
        }

        // 加速与限速逻辑,Launched状态之后的限速还没写
        if (Mathf.Abs(player.rb.velocity.x) < player.minMoveSpeed)//如果小于最小移动速度，直接设置为最小移动速度，确保起步流畅
        {
            player.rb.velocity = new Vector2(player.InputX * player.minMoveSpeed, player.rb.velocity.y);
        }
        else if (Mathf.Abs(player.rb.velocity.x) >= player.minMoveSpeed && Mathf.Abs(player.rb.velocity.x) < player.maxMoveSpeed)//如果未达到最大移动速度，加速
        {
            //player.rb.AddForce(new Vector2(inputX * player.moveSpeedAcc, 0));
            player.rb.velocity = new Vector2(Mathf.MoveTowards(player.rb.velocity.x, player.maxMoveSpeed * Mathf.Sign(player.rb.velocity.x), player.moveSpeedAcc * Time.fixedDeltaTime), player.rb.velocity.y);
        }
        else if (Mathf.Abs(player.rb.velocity.x) > player.maxMoveSpeed)//如果超过最大跑动速度，则限速
        {
            player.rb.velocity = new Vector2(Mathf.MoveTowards(player.rb.velocity.x, player.maxMoveSpeed * Mathf.Sign(player.rb.velocity.x), player.turnRoundBrakeDec * Time.fixedDeltaTime), player.rb.velocity.y);
        }
    }
    private void HandleFootstepSound()
    {
        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0)
        {
            // 核心：使用你 GroundedCheckerManager 里的 Ground LayerMask
            // 射线从脚下位置稍微往上一点发射，向下探测
            RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, 1.2f, player.groundedCheckerManager.Ground);
            Debug.Log("hit:" + hit);
            Debug.Log("hit:" + hit.collider.tag);
            if (hit.collider != null)
            {
                SoundEffectMgr.Instance.PlayFootstep(hit.collider.tag);
            }

            // 重置计时器
            stepTimer = stepInterval;
        }
    }
    public override void Exit()
    {
        base.Exit();
        // 关跑步特效
        player.playerVFXController.StopRunDust();
        // 重点：当停下来或者跳起来（离开RunState）时，重置音效索引
        SoundEffectMgr.Instance.ResetFootsteps();
    }
}