using UnityEngine;

public class ReleaseState : PlayerState
{
    public ReleaseState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    bool release = false;

    public override void Enter()
    {
        base.Enter();

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

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            release = true;
        }

    }

    public override void PhysicsUpdate()
    {
        // 物理帧保持静止，确保选方向时绝对精准
        player.rb.velocity = Vector2.zero;

        if (release)
        {
            Release();

            //判断是退出到地面状态还是空中状态
            if (!Input.GetKey(KeyCode.LeftControl))
            {
                if (player.groundedCheckerManager.isGrounded)
                {
                    // 要么run
                    if (Mathf.Abs(player.InputX) > 0.01f)
                    {
                        stateMachine.ChangeState(player.RunState);
                        return;
                    }
                    else//要么brake或者idle
                    {
                        stateMachine.ChangeState(player.BrakeState);

                        return;
                    }

                }
                else
                {
                    stateMachine.ChangeState(player.MidAirState);
                    return;
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        // 重力衰减
        player.postReleaseGravityTimer = player.postReleaseGravityReductionTime;

        //恢复时间
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;

        // 关闭箭头
        player.arrowInstance.SetActive(false);

        //将release设置为false
        release = false;    


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

    void Release()
    {
        //赋予玩家速度
        player.rb.velocity = player.currentStorage * player.releaseDir;

        //清空当前储量
        player.currentStorage = 0;
    }
}
