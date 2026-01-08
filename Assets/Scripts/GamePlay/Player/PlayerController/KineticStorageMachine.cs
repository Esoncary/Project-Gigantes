using UnityEngine;

public class KineticStorageMachine : MonoBehaviour
{
    private PlayerController player;
    private float explosionTimer; // 用于处理过载自爆的内部计时

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // 1. 获取物理速度的模长作为当前的目标
        float velocityMag = player.rb.velocity.magnitude;

        // 2. 核心判定：储量增长还是减少
        if (velocityMag > player.currentStorage)
        {
            // --- 增长逻辑 (充能模式) ---
            player.targetStorage = velocityMag;
            player.willDecrease = false;
            player.canDecrease = false;
            player.decreaseTimer = player.decreaseTime; // 只要在增长，就刷新衰减延迟计时器

            ApplyGrowth(player.targetStorage);
        }
        else if (velocityMag < player.currentStorage)
        {
            // --- 衰减逻辑 (维持/减少模式) ---
            // 目标值依然追随速度（暗红条掉下去），但 currentStorage（红条）准备进入延迟衰减
            player.targetStorage = velocityMag;

            if (!player.canDecrease)
            {
                player.willDecrease = true;
            }

            ApplyDecay();
        }

        // 3. 处理过载爆炸
        HandleOverloadLogic();
    }

    // 处理储量增长：使用你设计的三段阈值速度
    private void ApplyGrowth(float target)
    {
        float growthSpeed = 0f;

        if (target <= player.minIncreaseThreshold)
            growthSpeed = player.minIncreaseSpeed;
        else if (target <= player.midIncreaseThreshold)
            growthSpeed = player.midIncreaseSpeed;
        else
            growthSpeed = player.maxIncreaseSpeed;

        player.currentStorage = Mathf.MoveTowards(player.currentStorage, target, growthSpeed * Time.deltaTime);
    }

    // 处理储量减少：实现维持延迟（decreaseTime）
    private void ApplyDecay()
    {
        // 如果还在维持期，则走计时器
        if (player.willDecrease && !player.canDecrease)
        {
            player.decreaseTimer -= Time.deltaTime;
            if (player.decreaseTimer <= 0)
            {
                player.willDecrease = false;
                player.canDecrease = true;
            }
        }

        // 过了维持期，开始真正扣除数值
        if (player.canDecrease)
        {
            player.currentStorage = Mathf.MoveTowards(player.currentStorage, player.targetStorage, player.decreaseSpeed * Time.deltaTime);

            if (player.currentStorage <= player.targetStorage + 0.01f)
            {
                player.canDecrease = false;
            }
        }
    }

    // 处理过载爆炸逻辑
    private void HandleOverloadLogic()
    {
        if (player.currentStorage > player.maxStorage)
        {
            player.isOverLoaded = true;
            explosionTimer += Time.deltaTime;

            // 可以在这里加入视觉反馈，比如让玩家模型闪红
            // player.Anim.SetBool("Overloaded", true);

            if (explosionTimer >= player.explosionTime)
            {
                // 切换到死亡状态
                // 假设你已经有了 DieState，如果没有，先注释掉这行
                // player.StateMachine.ChangeState(player.DieState); 
                Debug.LogError("能量过载爆炸！");
            }
        }
        else
        {
            player.isOverLoaded = false;
            explosionTimer = 0;
        }
    }
}