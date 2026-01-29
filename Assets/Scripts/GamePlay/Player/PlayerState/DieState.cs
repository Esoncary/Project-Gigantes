using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : PlayerState
{
    //在死亡状态中，理应包含以下内容：广播死亡状态并且播放死亡动画，重新在复活点生成player

    public DieState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("玩家死了");
        // 1. 物理锁定：让玩家瞬间停下，且不再受重力或碰撞影响
        player.rb.velocity = Vector2.zero;
        player.rb.simulated = false; // 这会让玩家变成“幽灵”，不和任何东西碰撞

        // 2. 视觉处理：如果你没有死亡动画，可以暂时关掉渲染器
        player.GetComponentInChildren<SpriteRenderer>().enabled = false;

        // 触发死亡动画
        //player.PlayAnimation("Die");

        // 3. 广播信号：告诉所有人玩家死了
        GameEvents.PlayerDie?.Invoke();
    }

    public override void HandleInput()
    {
        // 在死亡状态下不处理任何输入
    }

    public override void LogicUpdate()
    {
        // 在死亡状态下不进行任何逻辑更新
    }

    public override void PhysicsUpdate()
    {
        // 在死亡状态下不进行任何物理更新
    }

    public override void Exit()
    {
        base.Exit();
        // 4. 重生时的清理：恢复物理模拟
        player.rb.simulated = true;
        player.GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void Reborn(Vector2 rebornPos)
    {
        // 1. 位置复位
        player.transform.position = rebornPos;
        player.rb.velocity = Vector2.zero;

        // 2. 状态重置：从 DieState 切回 IdleState
        // 这会自动触发 DieState.Exit() 里的 rb.simulated = true
        player.StateMachine.ChangeState(player.IdleState);

        // 3. 动力系统重置
        player.currentStorage = 0;
        player.targetStorage = 0;

        //4.恢复渲染器
        player.GetComponentInChildren<SpriteRenderer>().enabled = true;


    }
}
