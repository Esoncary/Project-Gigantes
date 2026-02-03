using GamePlay.IA.Base;
using GamePlay.Player.Interface;

namespace GamePlay.IA
{
    using UnityEngine;

    public class Trampoline : Switchable, IPlayerForce
    {
        [Header("基础弹力")]
        [Tooltip("基础弹力")]
        [SerializeField] private float baseForce = 20f;

        [Header("速度档位")]
        [Tooltip("第一档速度阈值：玩家在蹦床方向上的速度小于此值时使用 baseForce")]
        [SerializeField] private float lowSpeedThreshold = 25f;
        [Tooltip("第二档速度阈值：用于计算额外弹力的参考速度")]
        [SerializeField] private float highSpeedThreshold = 40f;
        [Tooltip("第三档速度阈值")]
        [SerializeField] private float moreHighSpeedThreshold = 60f;//为了关卡需要，我又增加了一档，增加速度直接使用extraforce的倍数
        [Tooltip("额外弹力：第二档时的最大额外弹力，额外弹力在 baseForce 的基础上计算")]
        [SerializeField] private float extraForce = 50f;

        [Header("其他设置")]
        [Tooltip("限制其他力的作用计时器，触碰后开始计时，应与实际效果时间一致")]
        [SerializeField] private float forceLimitTime = 1f;

        // 玩家实体触碰蹦床
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("蹦床触发 time: " + Time.timeAsDouble);
            var playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // 施加向上的弹力
                playerController.ApplyForce(this, forceLimitTime);
                //打开后释放计时器
                // playerController.postReleaseTimer = playerController.postReleaseTime;
                SoundEffectMgr.Instance.PlaySound("trampoline/trampoline");
            }
        }

        // IPlayerForce 实现
        public Vector2 CalculateVelocity(Vector2 currentVelocity, Vector2 playerPosition)
        {
            if (!IsActive)
            {
                Debug.Log("蹦床未激活");
                return currentVelocity;
            }

            // 获取弹射方向（基于 transform 上方）
            Vector2 bounceDirection = transform.up.normalized;

            // 计算玩家在弹射方向上的速度分量（投影）
            float playerSpeedInBounceDir = Mathf.Abs(Vector2.Dot(currentVelocity, bounceDirection));
            Debug.Log("力投影：" + playerSpeedInBounceDir);

            // 分档计算弹力
            float forceMagnitude;

            if (playerSpeedInBounceDir < lowSpeedThreshold)
            {
                // 第一档：低速玩家，提供基础弹力
                forceMagnitude = baseForce;
            }
            else if (playerSpeedInBounceDir < highSpeedThreshold)
            {
                // 第二档：中速玩家，根据速度提供额外弹力
                // 速度越大，额外弹力越小
                float speedRatio = (highSpeedThreshold - playerSpeedInBounceDir) / highSpeedThreshold;
                forceMagnitude = baseForce + extraForce * speedRatio;
            }
            else if (playerSpeedInBounceDir < moreHighSpeedThreshold )
            {
                // 超高速玩家，提供最大
                forceMagnitude = baseForce + extraForce;
            }
            else
            {
                forceMagnitude = baseForce + extraForce * 2f;
            }

                // 1. 计算玩家当前速度在蹦床方向上的分量（平行）
                float parallelComponent = Vector2.Dot(currentVelocity, bounceDirection);

            // 2. 计算垂直于蹦床方向的速度分量
            Vector2 perpendicularVelocity = currentVelocity - bounceDirection * parallelComponent;

            // 3. 新速度 = 弹射方向力 + 垂直方向保留的原有速度
            return bounceDirection * forceMagnitude + perpendicularVelocity;
        }

        /// <summary>
        /// 计算 x 轴分速度占总速度（向量长度）的比例
        /// </summary>
        private float GetXSpeedRatio(Vector2 velocity)
        {
            float totalSpeed = velocity.magnitude;
            if (totalSpeed < 0.01f)
                return 0f;
            return Mathf.Clamp01(Mathf.Abs(velocity.x) / totalSpeed);
        }

        public void OnSwitchOn()
        {
            Debug.Log("蹦床激活");
            IsActive = true;
        }

        public void OnSwitchOff()
        {
            Debug.Log("蹦床禁用");
            IsActive = false;
        }
    }
}