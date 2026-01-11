using GamePlay.Player.Interface;

namespace GamePlay.IA
{
    using UnityEngine;

    public class Trampoline : MonoBehaviour, IPlayerForce, ISwitchable
    {
        [Header("蹦床设置")]
        [Tooltip("蹦床弹力大小")]
        [SerializeField] private float bounceForce = 50f;
        [Tooltip("是否默认激活")]
        [SerializeField] private bool defaultIsOn = true;

        [Header("开关绑定")]
        [Tooltip("绑定的开关对象")]
        [SerializeField] private Switch boundSwitch;
        [Tooltip("是否跟随开关状态")]
        [SerializeField] private bool followSwitch;

        // 是否激活
        public bool IsActive { get; private set; } = true;
        
        private void Awake()
        {
            // 初始化激活状态
            // 将自身注册到开关
            if (followSwitch && boundSwitch != null)
            {
                IsActive = boundSwitch.IsOn;
                boundSwitch.RegisterSwitchable(this);
            }
            else
            {
                IsActive = defaultIsOn;
            }
        }

        
        // 玩家实体触碰蹦床
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // 施加向上的弹力
                playerController.ApplyForce(this);
            }
        }
        
        // IPlayerForce 实现
        // 设定力的类型与计算
        public ForceType ForceType => ForceType.External;
        public Vector2 CalculateVelocity(Vector2 currentVelocity)
        {
            if (IsActive)
            {
                // 仅修改垂直速度，水平速度保持不变
                return new Vector2(currentVelocity.x, bounceForce);
            }

            Debug.Log("蹦床未激活");
            return currentVelocity;
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