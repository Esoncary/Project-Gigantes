using UnityEngine;

public class BrakeState : PlayerState
{
    public BrakeState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    public override void Enter()
    {
        base.Enter();
        // 进入刹车状态时，可以播放一次“脚部摩擦地面的烟雾”粒子特效
        // Debug.Log("开始刹车...");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // --- 1. 灵敏度优先：如果刹车中途玩家又按了移动，立刻切回 Run ---
        if (Mathf.Abs(player.InputX) > 0.01f)
        {
            stateMachine.ChangeState(player.RunState);
            return;
        }

        // --- 2. 灵敏度优先：如果刹车中途按下跳跃，立刻起跳 ---
        if (player.jumpBufferTimer > 0 && player.canJump)
        {
            player.InitialJump(); 
            stateMachine.ChangeState(player.MidAirState);
            return;
        }

        // --- 3. 状态切换：如果脚下一空，切到空中 ---
        if (!player.groundedCheckerManager.isGrounded)
        {
            stateMachine.ChangeState(player.MidAirState);
            return;
        }

        // --- 3.5. 状态切换：释放 ---
        if (player.CanTriggerRelease())
        {
            player.ConsumeReleaseBuffer();
            stateMachine.ChangeState(player.ReleaseState);
            return;
        }

        // --- 3.6. 状态切换：死亡（自杀键）---
        if (player.DieInputDown)
        {
            stateMachine.ChangeState(player.DieState);
            return;
        }

        // --- 4. 状态切换：速度足够慢了，正式进入 Idle ---
        // 使用 Mathf.Abs 确保向左向右滑动都能正确检测
        if (Mathf.Abs(player.rb.velocity.x) < 0.01)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // --- 5. 核心物理逻辑：施加刹车力 ---
        // 这里的刹车力可以比普通的 idle 摩擦力更大一点，体现“刹车”的动作感
        if (Mathf.Abs(player.rb.velocity.x) > 0.01f)
        {
            //使当前速度趋向于0
            player.rb.velocity = new Vector2(Mathf.MoveTowards(player.rb.velocity.x, 0, player.turnRoundBrakeDec * Time.fixedDeltaTime), player.rb.velocity.y);    
        }
        else
        {
            // 速度极其微小时，强行归零防止滑动
            player.rb.velocity = new Vector2(0, player.rb.velocity.y);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // 离开刹车状态，停止刹车音效或粒子
    }
}