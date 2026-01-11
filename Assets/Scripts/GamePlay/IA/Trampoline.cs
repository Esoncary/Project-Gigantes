using System;
using GamePlay.Player.Interface;

namespace GamePlay.IA
{
    using UnityEngine;

    public class Trampoline : MonoBehaviour, IPlayerForce
    {
        private float bounceForce = 50f; // 蹦床的弹力大小
        
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

        private void OnTriggerExit(Collider other)
        {
            throw new NotImplementedException();
        }
        
        // IPlayerForce 实现
        // 设定力的类型与计算
        public ForceType ForceType => ForceType.External;
        public Vector2 CalculateVelocity(Vector2 currentVelocity)
        {
            // 仅修改垂直速度，水平速度保持不变
            return new Vector2(currentVelocity.x, bounceForce);
        }
    }
}