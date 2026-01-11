using System;
using GamePlay.Player.Interface;
using UnityEngine;

namespace GamePlay.IA
{
    public class Fan : Switchable, IPlayerForce
    {
        [Header("风车设置")]
        [Tooltip("风力大小")]
        [SerializeField] private float windForce = 5f;
        [Tooltip("风力方向")]
        [SerializeField] private Direction windDirection = Direction.UP;
        [Tooltip("风力无衰减的范围")]
        [SerializeField] private float windRange = 15f;
        [Tooltip("风力最大作用距离")]
        [SerializeField] private float maxDistance = 15f;
        [Tooltip("角色自惯性")]
        [SerializeField] private float selfInertial = 0.5f;
        
        public ForceType ForceType => ForceType.External;

        private void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ApplyForce(this);  // 每帧都在受力区时调用
            }
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // var playerController = collision.GetComponent<PlayerController>();
            // if (playerController != null)
            // {
            //     // 施加向上的弹力
            //     playerController.ApplyForce(this);
            // }
        }
        
        private void OnTriggerExit2D(Collider2D collision)
        {
            // Debug.Log("进入风扇范围");
            // var playerController = collision.GetComponent<PlayerController>();
            // if (playerController != null)
            // {
            //     // 施加风力
            //     playerController.ApplyForce(this);
            // }
        }

        public Vector2 CalculateVelocity(Vector2 currentVelocity, Vector2 playerPosition)
        {
            if (!IsActive) return currentVelocity;
            
            // 计算玩家距离风扇的位置
            float distance = Vector2.Distance(transform.position, playerPosition);
            float intensity = Mathf.Clamp01(1 - distance / maxDistance);
            
            if (intensity <= 0) return currentVelocity;

            Vector2 forceDirection = Vector2.zero;
            switch (windDirection)
            {
                case Direction.UP:
                    forceDirection = Vector2.up;
                    break;
                case Direction.DOWN:
                    forceDirection = Vector2.down;
                    break;
                case Direction.LEFT:
                    forceDirection = Vector2.left;
                    break;
                case Direction.RIGHT:
                    forceDirection = Vector2.right;
                    break;
            }

            Debug.Log("风扇计算的力：" +  currentVelocity + forceDirection * (windForce * intensity));
            return new Vector2(
                forceDirection.x == 0 ? currentVelocity.x : currentVelocity.x * selfInertial + forceDirection.x * (windForce * intensity), 
                forceDirection.y == 0 ? currentVelocity.y : currentVelocity.y * selfInertial + forceDirection.y * (windForce * intensity));
        }

        public void OnSwitchOn()
        {
            IsActive = true;
        }

        public void OnSwitchOff()
        {
            IsActive = false;
        }
    }

    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}