using GamePlay.IA.Base;
using UnityEngine;

namespace GamePlay.Pickups
{
    public class Key : Switch, IPickUp
    {
        private bool _isPickUp;

        protected override void Start()
        {
            // 确保钥匙初始不发送通知，防止一开始就误触发
        }

        public void PickUpEffect(PlayerController player)
        {
            transform.SetParent(player.transform);
            transform.localPosition = Vector2.zero;
        }

        // 【新增】供 Door 调用的方法：当钥匙被成功使用时
        public void OnKeyUsed()
        {
            Debug.Log("钥匙被消耗了");
            // 彻底隐藏钥匙，但不要 Destroy，因为 Door 的 boundSwitches 还需要读取它的数据
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 1. 只保留“被玩家捡起”的逻辑
            if (!_isPickUp && collision.TryGetComponent<PlayerController>(out var player))
            {
                Debug.Log("玩家捡起钥匙");
                _isPickUp = true;
                PickUpEffect(player);

                // 捡起后，关闭 Sprite，保留 GameObject 激活状态，
                // 这样 Door 才能通过 GetComponentsInChildren 找到它
                GetComponent<SpriteRenderer>().enabled = false;

                // 捡起后，把 Collider 关掉，避免它作为玩家子物体时干扰玩家的碰撞检测
                var col = GetComponent<Collider2D>();
                if (col) col.enabled = false;
            }

            // 2. 删除“钥匙撞门”的逻辑
            // 因为现在是门主动来找钥匙，不需要钥匙去撞门了
        }
    }
}