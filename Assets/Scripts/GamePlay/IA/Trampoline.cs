using System;

namespace GamePlay.IA
{
    using UnityEngine;

    public class Trampoline : MonoBehaviour
    {
        private float bounceForce = 50f; // 蹦床的弹力大小
        
        // 玩家实体触碰蹦床
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 获取玩家的 Rigidbody2D 组件
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            
            if (rb != null)
            {
                rb.velocity = Vector2.up * bounceForce;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            throw new NotImplementedException();
        }
    }
}