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
        //这里处理计时器
        //关掉变量跳跃计时器
        if ((Input.GetKeyUp(KeyCode.Space) && player.varJumpTimer >= 0) || player.varJumpTimer <= 0) 
        {
            player.canVarJump = false;
            player.varJumpTimer = -1f; 
        }


        // --- 根据垂直速度切换上升/下落动画 ---
        if (player.postReleaseTimer > 0)//如果处于后释放期间
        {
            player.rb.gravityScale = player.defaultGravityScale * player.postReleaseGravityScale;//使用后释放的中立缩放系数
        }
        //如果不处于后释放期间，有两种情况，一是上升，二是下落
        else if (player.rb.velocity.y > 0.1f && !player.canVarJump)//player处于上升阶段且不在变量跳跃期间
        {
            player.PlayAnimation("Jump_Up");
            if (Input.GetKey(KeyCode.Space))
            {
                player.rb.gravityScale = player.defaultGravityScale * 2f; // 持续按住跳跃键时使用正常重力
            }
            else
            {
                player.rb.gravityScale = player.defaultGravityScale * 4f; // 松开跳跃键时增加重力加速度
            }
        }
        else if (player.rb.velocity.y < -0.1f)
        {
            player.PlayAnimation("Fall_Down");
            player.rb.gravityScale = player.defaultGravityScale * 3.5f; // 下落时增加重力加速度
        }
        

        //---1.空中跳跃-- -
        if (player.jumpBufferTimer > 0 && player.canJump )
        {
            player.InitialJump();
            player.canJump = false; // 落地前只能跳一次
            stateMachine.ChangeState(player.MidAirState);
            return;
        }

        // --- 2. 状态切换：落地 ---
        if (player.groundedCheckerManager.isGrounded && player.rb.velocity.y <= 0.01f)
        {
            // 如果落地时还有较大的水平输入，直接进入 Run，否则进入 Brake 或 Idle
            if (Mathf.Abs(player.InputX) > 0.01f)
            {
                player.canJump = true;//恢复跳跃次数
                stateMachine.ChangeState(player.RunState);
            }
            else
            {
                player.canJump = true;//恢复跳跃次数
                stateMachine.ChangeState(player.BrakeState);

                return;
            }
                
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
        AirMovement();

        // --- 6. 持续跳跃加力 (长按跳得高) ---
        if (player.canVarJump)
        {
            VarJump();
        }
        

        // --- 7. 到达顶峰时的滞空效果
        HangInMidAir();
    }

    private void AirMovement()
    {
        //我们这里有两种情况，一种是正常情况，一种是后释放情况。这里通过一个函数来实现，用currentMoveSpeedAccScale这个变量来控制两种情况。如果是正常情况，这个变量就是1；如果是后释放情况，这个变量会小于1然后逐渐恢复到1。
        if (player.postReleaseTimer <= 0)
        {
            // 转向处理
            if (Mathf.Abs(player.rb.velocity.x) > 0.1f && Mathf.Sign(player.rb.velocity.x) != player.InputX)
            {
                player.TurnRoundBrake(player.turnRoundBrakeDec * player.currentMoveSpeedAccScale);
            }

            // 加速与限速逻辑,Launched状态之后的限速还没写
            if (Mathf.Abs(player.rb.velocity.x) < player.minMoveSpeedInMidAir)//如果小于最小移动速度，直接设置为最小移动速度，确保起步流畅
            {
                player.rb.velocity = new Vector2(player.InputX * player.minMoveSpeedInMidAir, player.rb.velocity.y);
            }
            else if (Mathf.Abs(player.rb.velocity.x) >= player.minMoveSpeedInMidAir && Mathf.Abs(player.rb.velocity.x) < player.maxMoveSpeedInMidAir)//如果未达到最大移动速度，加速
            {
                //player.rb.AddForce(new Vector2(inputX * player.moveSpeedAcc, 0));
                player.rb.velocity = new Vector2(Mathf.MoveTowards(player.rb.velocity.x, player.maxMoveSpeedInMidAir * Mathf.Sign(player.rb.velocity.x), player.moveSpeedAccInMidAir * player.currentMoveSpeedAccScale * Time.fixedDeltaTime), player.rb.velocity.y);
            }
            else if (Mathf.Abs(player.rb.velocity.x) > player.maxMoveSpeedInMidAir)//如果超过最大跑动速度，则限速
            {
                player.rb.velocity = new Vector2(Mathf.MoveTowards(player.rb.velocity.x, player.maxMoveSpeedInMidAir * Mathf.Sign(player.rb.velocity.x), player.turnRoundBrakeDec * Time.fixedDeltaTime), player.rb.velocity.y);
            }
        }
        
    }

    private void VarJump()
    {
        // 这里的逻辑对应你之前的 varJumpTimer
        //player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpSpeedInitial);
        player.rb.gravityScale = 0; // 在变量跳跃期间取消重力影响

    }

    private void HangInMidAir()//滞空处理
    {
        // 当垂直速度接近 0（到达抛物线顶端）时，减小重力产生滞空感
        if (Mathf.Abs(player.rb.velocity.y) < player.gravityContractionThreshold)
        {
            player.rb.gravityScale = player.gravityContractionScale * player.defaultGravityScale;
        }
        //else
        //{
        //    player.rb.gravityScale = player.defaultGravityScale; // 恢复正常重力 //由于整个跳跃过程的重力都被控制了，所以不需要else恢复重力
        //}
    }

    public override void Exit()
    {
        base.Exit();
        // 离开空中状态时，务必恢复重力常数，防止影响其他状态
        player.rb.gravityScale =  player.defaultGravityScale;
    }
}