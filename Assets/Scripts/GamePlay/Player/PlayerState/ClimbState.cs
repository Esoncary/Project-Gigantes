using UnityEngine;

public class ClimbState : PlayerState
{
    public ClimbState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    
    public override void Enter()
    {
        base.Enter();

        // 1. 直接在这里执行吸附（逻辑内聚）
        SnapToWallInternal();

        // 2. 物理锁定
        player.rb.velocity = Vector2.zero;
        player.rb.gravityScale = 0;

        // 3. 计时器
        player.climbTimer = player.climbTime;
    }

    public override void HandleInput()
    {
        base.HandleInput();

        // 蹬墙跳判定
        if (player.JumpInputDown)
        {
            WallJumpInternal(); // 逻辑也写在下面
            stateMachine.ChangeState(player.MidAirState);
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 使用你提到的子物体检测器变量进行判断
        bool touchingWall = player.isOnLeftWall || player.isOnRightWall;

        player.climbTimer -= Time.deltaTime;

        if (player.climbTimer <= 0 || Input.GetKeyUp(KeyCode.J) || player.isGrounded || !touchingWall)
        {
            stateMachine.ChangeState(player.MidAirState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // 持续锁定，防止任何抖动
        player.rb.velocity = Vector2.zero;
    }

    public override void Exit()
    {
        base.Exit();
        player.rb.gravityScale = player.defaultGravityScale;
    }

    #region 内部私有逻辑 (只在攀爬状态下使用的工具函数)

    private void SnapToWallInternal()
    {
        // 1. 确定方向：从 PlayerController 获取子物体的检测结果
        float direction = player.isOnRightWall ? 1f : -1f;
        Vector2 wallDir = new Vector2(direction, 0);

        // 2. 射线探测（从中心向墙的方向）
        // 使用 player.col 获取身体碰撞体的尺寸
        float rayLength = player.col.bounds.extents.x + 0.2f;

        // 注意：这里需要访问 player 身上定义的 Ground LayerMask 
        RaycastHit2D hit = Physics2D.Raycast(player.rb.position, wallDir, rayLength, player.groundedCheckerManager.Ground);

        if (hit.collider != null)
        {
            float halfWidth = player.col.bounds.extents.x;
            float skin = 0.01f; // 物理皮肤间距

            float targetX = hit.point.x - (halfWidth * direction) + (skin * direction);

            // 瞬间物理同步
            player.rb.position = new Vector2(targetX, player.rb.position.y);
            player.rb.velocity = Vector2.zero;
        }
    }

    private void WallJumpInternal()
    {
        // 确定跳出方向
        float jumpDirX = player.isOnRightWall ? -1f : 1f;

        // 组合 45 度向量
        Vector2 jumpVec = new Vector2(jumpDirX * player.wallJumpDirection.x, player.wallJumpDirection.y).normalized;

        // 爆发位移
        player.rb.velocity = jumpVec * player.wallJumpSpeed;

        // 开启控制器上的输入锁定计时器
        player.inputLockTimer = player.inputLockTime;
    }

    #endregion
}