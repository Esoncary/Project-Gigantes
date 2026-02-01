using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchedState : PlayerState
{
    public LaunchedState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }
    
    private float launchTimer;          // 已经历的衰减时间
    private float totalLaunchTime;      // 总衰减时间（基于能量百分比）
    private Vector2 launchDirection;    // 发射时的释放方向
    
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
        
        // 保存发射方向
        launchDirection = player.releaseDir.normalized;

        // 计算持续时间：基于能量百分比
        float energyRatio = player.currentStorage / player.maxStorage;

        //260130：此处我做了修改：因为要引入增强剂（短时间增加最大储量），所以要让最大储量的值本身也参与到finalspeed的计算中，我多乘了一个系数（当前最大储量/默认最大储量）
        float finalSpeed = player.baseSpeed + (player.maxSpeed - player.baseSpeed) * energyRatio * ( player.maxStorage / 70f);//f是我在inspector中设置的值


        totalLaunchTime = player.releaseDragTime * energyRatio;
        launchTimer = 0f;

        // 赋予初速度
        player.rb.velocity = finalSpeed * launchDirection;

        // 清空能量
        player.currentStorage = 0;

        //目标值和当前值进入一个极短的冷却时间
        player.coolerTimer = 0.5f;

        //开启释放特效
        float angle = Mathf.Atan2(player.releaseDir.y, player.releaseDir.x) * Mathf.Rad2Deg;//计算特效角度
        if (player.transform.localScale.x < 0)
        {
            angle += 180f;
        }
        player.playerVFXController.PlayRelease(angle);
    }

    public override void HandleInput()
    {
        //在launched的状态中，屏蔽所有玩家的输入
    }

    public override void LogicUpdate()
    {
        // 1. 计时器判定：如果时间到了，恢复控制权
        // if (player.speedLimitOffTimer <= 0)
        // {
        //     player.speedLimitOffTimer = -1; //关闭speedLimitOff计时器
        //     stateMachine.ChangeState(player.MidAirState);
        //     return;
        // }

        //// 2. 碰撞判定：如果高速弹射中撞到了墙，通常应该提前结束锁定
        //if (player.isGrounded )
        //{

        //    stateMachine.ChangeState(player.RunState);
        //    return;
        //}
        //if (player.canClimb)
        //{
        //    stateMachine.ChangeState(player.MidAirState);
        //    return;
        //}

        UpdateLaunchedDirInAnimation(launchDirection);
    }

    public override void PhysicsUpdate()
    {
        if (ShouldExitLaunchingPhase())
        {
            TransitionToNextState();
        }
        else
        {
            launchTimer += Time.fixedDeltaTime;
            // player.currentStorage = 0;
            // player.targetStorage = 0;
            UpdateLaunchingPhysics();
        }
    }
    
    /**
     * 按帧计算角色速度
     * 分 X/Y 轴分别处理
     */
    private void UpdateLaunchingPhysics()
    {
        Vector2 currentVelocity = player.rb.velocity;
        float dragFactor = Mathf.Exp(-player.releaseDragCoefficient * Time.fixedDeltaTime);

        // === X轴 ===
        float newX = currentVelocity.x * dragFactor;
        // 若当前速度 >= 阈值，且衰减后 < 阈值
        if (Mathf.Abs(currentVelocity.x) >= player.launchXMinSpeed &&
            Mathf.Abs(newX) < player.launchXMinSpeed)
        {
            // 判断玩家输入是否与发射方向同向
            float launchDirX = Mathf.Sign(launchDirection.x);
            bool isInputSameDirection = Mathf.Sign(player.InputX) == launchDirX && Mathf.Abs(player.InputX) > 0.01f;

            if (isInputSameDirection)
            {
                // 玩家需要向当前喷射的方向运动，速度不应该低于正常位移的速度
                newX = player.launchXMinSpeed * launchDirX;
            }
        }

        // === Y轴 ===
        float newY;
        newY = currentVelocity.y * dragFactor;

        player.rb.velocity = new Vector2(newX, newY);
    }
    
    /**
     * 是否退出释放状态
     */
    private bool ShouldExitLaunchingPhase()
    {
        // 时间结束
        if (launchTimer >= totalLaunchTime)
            return true;
        

        // 落地
        if (player.isGrounded)
            return true;

        return false;
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

    public override void Exit()
    {
        base.Exit();
        if (launchDirection.y >= 0)
        {
            player.launchedStagnationTimer = player.launchedStagnationTime;
        }
        //避免释放时的速率立马影响到装置
        player.targetStorage = 0;
        player.currentStorage = 0;
    }

    //这里处理播放动画的变量，根据方向决定播放哪个动画
    public void UpdateLaunchedDirInAnimation(Vector2 launchDir)
    {
        // 1. 必须归一化，确保向量长度为 1，这样判定才准确
        Vector2 dir = launchDir.normalized;

        // 2. 根据面朝向转换坐标
        float LaunchedX = player.isFacingRight ? dir.x : -dir.x;
        float LaunchedY = dir.y;

        // 3. 传给 Animator 里的参数
        player.Anim.SetFloat("LaunchedX", LaunchedX);
        player.Anim.SetFloat("LaunchedY", LaunchedY);
    }
}
