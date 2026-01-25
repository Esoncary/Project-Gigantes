using UnityEngine;

public class ReleaseState : PlayerState
{
    public ReleaseState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    public override void Enter()
    {
        base.Enter();

        if (player.releaseCoolingTimer > 0)
        {
            Debug.Log("释放器处于 CD 中");
            TransitionToNextState();
            return;
        }
        
        // 1. 开启子弹时间
        Time.timeScale = player.timeScaleReleasing;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // 必须同步修改物理步长，防止卡顿

        // 2. 物理冻结：防止在瞄准时掉下去
        player.rb.velocity = Vector2.zero;
        player.rb.gravityScale = 0;

        // 3. 显示 UI 箭头
        if (player.arrowInstance != null)
        {
            player.arrowInstance.SetActive(true);
            // 初始默认方向：如果没有输入过，默认指向上
            if (player.releaseDir == Vector2.zero) player.releaseDir = Vector2.up;
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Aim();
        }
        
        // 释放
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (player.currentStorage < player.releaseThreshold)
            {
                TransitionToNextState();
            }
            else
            {
                ExecuteRelease();
            }
        }
    }

    public override void PhysicsUpdate()
    {
        // 物理帧保持静止，确保选方向时绝对精准
        player.rb.velocity = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();

        //恢复时间
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;

        // 关闭箭头
        if (player.arrowInstance != null)
            player.arrowInstance.SetActive(false);
        
        // 恢复重力
        if (player.rb.gravityScale == 0)
            player.rb.gravityScale = player.defaultGravityScale;
    }

    /**
     * 状态机切换
     */
    private void TransitionToNextState()
    {
        if (player.isGrounded)
        {
            // 地面：根据输入决定 Brake 或 Run
            if (Mathf.Abs(player.InputX) > 0.01f)
                stateMachine.ChangeState(player.RunState);
            else
                stateMachine.ChangeState(player.BrakeState);
        }
        else
        {
            // 空中：切换到 MidAirState
            stateMachine.ChangeState(player.MidAirState);
        }
    }


    void Aim()
    {
        // --- 方案 A：鼠标指向 (PC 玩家最精准的操作方式) ---
        player.releaseDir = player.MouseDir;

        //// --- 方案 B：手柄摇杆 / 键盘 (使用 GetAxis 而不是 GetAxisRaw) ---
        //// GetAxis 会有 0 到 1 之间的中间值，手柄摇杆可以实现 360 度
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");

        //if (Mathf.Abs(h) > 0.2f || Mathf.Abs(v) > 0.2f)
        //{
        //    dir = new Vector2(h, v).normalized;
        //}

        // --- 最终方向锁定 ---
        // 只有当有输入时才更新方向，否则保持上一帧的方向 (类似奥日的方向记忆)
        //if (player.releaseDir != Vector2.zero)
        //{
        //    player.releaseDir = dir;
        //}


        //暂时把箭头方向（视觉效果）和转向逻辑放在一起了
        if (player.arrowInstance != null)
        {
            // 使用 Atan2 算出弧度并转为角度
            float angle = Mathf.Atan2(player.releaseDir.y, player.releaseDir.x) * Mathf.Rad2Deg;

            // 修正：你的图片默认向上，所以需要减去 90 度偏移
            // 如果你的箭头尖端指向右，则不需要这个 -90f
            player.arrowInstance.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
    
    /**
     * 进入发射状态
     */
    private void ExecuteRelease()
    {
        player.releaseCoolingTimer = player.releaseCoolingLimit;
        // 隐藏箭头
        if (player.arrowInstance != null)
            player.arrowInstance.SetActive(false);
        
        stateMachine.ChangeState(player.LaunchedState);
    }
}
