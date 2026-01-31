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
        //这里先处理动画和音效
        player.Anim.SetFloat("VelocityY", player.rb.velocity.y);

        //这里处理计时器
        //关掉变量跳跃计时器
        if ((Input.GetKeyUp(KeyCode.Space) && player.varJumpTimer >= 0) || player.varJumpTimer <= 0) 
        {
            player.canVarJump = false;
            player.varJumpTimer = -1f; 
        }
        // 有两种情况，一是上升，二是下落
        if (player.rb.velocity.y > 0.1f && !player.canVarJump)//player处于上升阶段且不在变量跳跃期间
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
        
        if (player.forceLimitTimer > 0)
        {
            // 处于力限制状态下，恢复正常重力
            player.rb.gravityScale = player.defaultGravityScale * 4f;
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
        // 是否处于力限制状态
        bool isInForceLimit = player.forceLimitTimer > 0;
        // 根据当前 x 轴分速度/总速度的比例计算机动性削弱系数：x 轴占比越大，操控越弱
        float mobilityScale = isInForceLimit
            ? Mathf.Lerp(0.2f, 1f, 1f - GetXSpeedRatio())
            : 1f;

        //我们这里有两种情况，一种是正常情况，一种是后释放情况。这里通过一个函数来实现，用currentMoveSpeedAccScale这个变量来控制两种情况。如果是正常情况，这个变量就是1；如果是后释放情况，这个变量会小于1然后逐渐恢复到1。
        // 转向处理
        // if (Mathf.Abs(player.rb.velocity.x) > 0.1f && Mathf.Sign(player.rb.velocity.x) != player.InputX)
        // {
        //     player.TurnRoundBrake(player.turnRoundBrakeDec);
        // }
        // Debug.Log("0");
        // // 加速与限速逻辑,Launched状态之后的限速还没写
        // if (Mathf.Abs(player.rb.velocity.x) < player.minMoveSpeedInMidAir)//如果小于最小移动速度，直接设置为最小移动速度，确保起步流畅
        // {
        //     Debug.Log("1");
        //     // player.rb.velocity = new Vector2(player.InputX * player.minMoveSpeedInMidAir, player.rb.velocity.y);
        // }
        // else if (Mathf.Abs(player.rb.velocity.x) >= player.minMoveSpeedInMidAir && Mathf.Abs(player.rb.velocity.x) < player.maxMoveSpeedInMidAir)//如果未达到最大移动速度，加速
        // {
        //     Debug.Log("2");
        //     //player.rb.AddForce(new Vector2(inputX * player.moveSpeedAcc, 0));
        //     player.rb.velocity = new Vector2(Mathf.MoveTowards(player.rb.velocity.x, player.maxMoveSpeedInMidAir * Mathf.Sign(player.rb.velocity.x), player.moveSpeedAccInMidAir *  Time.fixedDeltaTime), player.rb.velocity.y);
        // }
        // else

        // 如果超过最大跑动速度，则限速
        if (Mathf.Abs(player.rb.velocity.x) > player.maxMoveSpeedInMidAir)
        {
            player.rb.velocity = new Vector2(Mathf.MoveTowards(player.rb.velocity.x, player.maxMoveSpeedInMidAir * Mathf.Sign(player.rb.velocity.x), player.turnRoundBrakeDec * Time.fixedDeltaTime), player.rb.velocity.y);
        }
        bool hasInput = Mathf.Abs(player.InputX) > 0.01f;
        if (!hasInput)
        {
            // 无输入：保留惯性，只应用轻微阻力
            float currentSpeedX = player.rb.velocity.x;
            float newSpeedX = Mathf.MoveTowards(
                currentSpeedX,
                0,
                player.airDragWithoutInput * Time.fixedDeltaTime
            );
            player.rb.velocity = new Vector2(newSpeedX, player.rb.velocity.y);
        }
        else
        {
            // 有输入：提供操控感（力限制期间操控性降低）

            // 1. 转向刹车：速度方向与输入相反时快速减速
            if (Mathf.Abs(player.rb.velocity.x) > 0.1f &&
                Mathf.Sign(player.rb.velocity.x) != player.InputX)
            {
                player.TurnRoundBrake(player.airTurnBrakeForce * mobilityScale);
            }
            // 2. 正常加速：向输入方向响应（力限制期间响应度降低）
            else
            {
                float targetSpeed = player.maxMoveSpeedInMidAir * player.InputX;
                float newX = Mathf.MoveTowards(
                    player.rb.velocity.x,
                    targetSpeed,
                    player.airResponsiveness * mobilityScale * Time.fixedDeltaTime
                );
                player.rb.velocity = new Vector2(newX, player.rb.velocity.y);
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

    /// <summary>
    /// 计算 x 轴分速度占总速度（向量长度）的比例
    /// </summary>
    private float GetXSpeedRatio()
    {
        Vector2 velocity = player.rb.velocity;
        float totalSpeed = velocity.magnitude;
        if (totalSpeed < 0.01f)
            return 0f;
        return Mathf.Clamp01(Mathf.Abs(velocity.x) / totalSpeed);
    }
}