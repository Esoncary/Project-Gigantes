using System;
using GamePlay.IA.Base;
using GamePlay.Player.Interface;
using UnityEngine;

namespace GamePlay.IA
{
    public class Fan : Switchable, IPlayerForce
    {
        [Header("风车设置")] [Tooltip("风力大小")] [SerializeField]
        private float windForce = 5f;

        [Tooltip("风力方向")] [SerializeField] private Direction windDirection = Direction.UP;
        [Tooltip("风力最大作用距离(相对于风场实体)")] [SerializeField] private float maxDistance = 15f;
        [Tooltip("风力边缘过渡区")] [SerializeField] private float fadeDistance = 3f;
        [Tooltip("角色自惯性")] [SerializeField] private float selfInertial = 0.5f;

        // 若持续处于作用范围内
        private void OnTriggerStay2D(Collider2D collision)
        {
            var player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ApplyForce(this, 0); // 每帧都在受力区时调用
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
        }

        public Vector2 CalculateVelocity(Vector2 currentVelocity, Vector2 playerPosition)
        {
            if (!IsActive) return currentVelocity;

            // 计算玩家距离风扇的位置
            float distance = Vector2.Distance(transform.position, playerPosition);
            
            // 边缘距离时风力衰减
            float effectiveRange = maxDistance - fadeDistance;
            float t = distance <= effectiveRange
                ? 1f // 非边缘区域
                : Mathf.Clamp01((maxDistance - distance) / fadeDistance);  // 边缘区域：平滑衰减
            float intensity = t * t * (3f - 2f * t);
            float lateralDrag = 1f - (intensity * 0.1f); // 垂直方向上阻力

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

            Debug.Log("当前风场距离" + distance);

            Vector2 newVelocity = new Vector2(
                forceDirection.x == 0
                    ? currentVelocity.x * lateralDrag // 垂直方向
                    : currentVelocity.x + forceDirection.x * (windForce * intensity), // 风场方向
                forceDirection.y == 0
                    ? currentVelocity.y * lateralDrag // 垂直方向
                    : currentVelocity.y + forceDirection.y * (windForce * intensity) // 风场方向
            );

            // 计算当前速度在风力方向上的投影
            float currentProjectedSpeed = Vector2.Dot(currentVelocity, forceDirection);
            float maxWindSpeed = windForce * intensity;

            // 如果当前投影速度已经达到或超过风力最大值，则保持当前速度不变
            return currentProjectedSpeed >= maxWindSpeed
                ? currentVelocity
                : newVelocity;
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