using GamePlay.IA.Base;
using GamePlay.Player.Interface;

namespace GamePlay.IA
{
    using UnityEngine;

    public class Trampoline : Switchable, IPlayerForce
    {
        [Header("蹦床设置")]
        [Tooltip("蹦床弹力大小")]
        [SerializeField] private float bounceForce = 50f;
        
        // 玩家实体触碰蹦床
        private void OnTriggerEnter2D(Collider2D collision)
        {
            var playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // 施加向上的弹力
                playerController.ApplyForce(this);
                //打开后释放计时器
                playerController.postReleaseTimer = playerController.postReleaseTime;
            }
        }
        
        // IPlayerForce 实现
        public Vector2 CalculateVelocity(Vector2 currentVelocity, Vector2 playerPosition)
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