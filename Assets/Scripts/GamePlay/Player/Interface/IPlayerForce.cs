using UnityEngine;

namespace GamePlay.Player.Interface
{
    public interface IPlayerForce
    {
        // 根据当前速度计算施加力后的新速度
        Vector2 CalculateVelocity(Vector2 currentVelocity);
        ForceType ForceType { get; }
    }
    
    // 力的类型：内部力（如加速）或外部力（如风力、碰撞力）
    public enum ForceType
    {
        Internal,
        External
    }
}