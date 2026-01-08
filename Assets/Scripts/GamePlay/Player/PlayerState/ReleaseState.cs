using UnityEngine;

public class ReleaseState : PlayerState
{
    public ReleaseState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

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
            if (player.releaseDirection == Vector2.zero) player.releaseDirection = Vector2.up;
        }
    }

    public override void HandleInput()
    {
        base.HandleInput();

        // 4. 瞄准逻辑：读取 WASD/摇杆方向
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(h) > 0.05f || Mathf.Abs(v) > 0.05f)
        {
            player.releaseDirection = new Vector2(h, v).normalized;
        }

        // 5. 状态退出判定：松开释放键 (LeftControl)
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            stateMachine.ChangeState(player.MidAirState); // 退出到空中状态
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 实时旋转箭头
        if (player.arrowInstance != null)
        {
            float angle = Mathf.Atan2(player.releaseDirection.y, player.releaseDirection.x) * Mathf.Rad2Deg;
            // 如果你之前的箭头偏了 90 度，这里记得加上那个 offset
            player.arrowInstance.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
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

        // 6. 恢复时间常数
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;

        // 7. 恢复重力
        player.rb.gravityScale = 1.0f;

        // 8. 【核心物理输出】：瞬间弹射位移
        // 速度 = 方向 * 储能装置里的当前数值
        // 注意：这里建议直接改速度，因为这属于瞬间爆发
        player.rb.velocity = player.releaseDirection * player.currentStorage;

        // 9. 资源清空与锁定
        player.currentStorage = 0; // 清空储能
        player.inputLockTimer = player.inputLockTime; // 开启输入锁定，防止被空中移动逻辑干扰位移

        // 10. 隐藏箭头
        if (player.arrowInstance != null) player.arrowInstance.SetActive(false);

        // Debug.Log("释放成功！速度为：" + player.rb.velocity.magnitude);
    }
}
