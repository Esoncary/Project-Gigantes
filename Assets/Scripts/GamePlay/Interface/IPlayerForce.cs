using UnityEngine;

namespace GamePlay.Player.Interface
{
    /**
     * 对玩家施加力的接口
     */
    public interface IPlayerForce
    {
        // 根据当前速度计算施加力后的新速度
        // 入参：当前速度，玩家位置
        Vector2 CalculateVelocity(Vector2 currentVelocity, Vector2 playerPosition);
    }
}