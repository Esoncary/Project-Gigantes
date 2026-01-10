using UnityEngine;

public abstract class PlayerState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;
    protected string animName;

    
    protected float startTime;//状态开始时间 （用来控制冲刺时长、受击硬直、限速等）
    

    public PlayerState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.animName = _animName;
    }

    // 进入状态：处理初始化、播动画、放特效
    public virtual void Enter()
    {
        player.PlayAnimation(animName); // 调用控制器里的网关函数
        startTime = Time.time;
        // Debug.Log("进入状态: " + this.GetType().Name);
    }

    // 退出状态：处理善后、关闭特效、恢复重力
    public virtual void Exit()
    {
        // Debug.Log("退出状态: " + this.GetType().Name);
    }

    // 逻辑帧：Update 里的输入获取
    public virtual void HandleInput()
    {
    }

    // 逻辑帧：Update 里的状态切换判定
    public virtual void LogicUpdate()
    {
    }

    // 物理帧：FixedUpdate 里的力学计算
    public virtual void PhysicsUpdate()
    {
        
    }



    // 物理反馈：由 PlayerController 转发碰撞事件
    public virtual void OnCollisionEnter(Collision2D collision)
    {
    }

    // 触发器反馈：由 PlayerController 转发触发事件
    public virtual void OnTriggerEnter(Collider2D collider)
    {
    }
}
