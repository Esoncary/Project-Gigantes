using GamePlay.IA;
using GamePlay.IA.Base;
using UnityEngine;

namespace GamePlay.Pickups
{
    /**
     * 钥匙，用于开门
     */
    public class Key : Switch, IPickUp
    {
        bool _isPickUp;

        public void PickUpEffect(PlayerController player)
        {
            // 将钥匙附加到玩家身上
            transform.SetParent(player.transform);
            transform.localPosition = Vector2.zero;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 玩家捡起钥匙
            if (!_isPickUp && collision.TryGetComponent<PlayerController>(out var player) && player)
            {
                Debug.Log("玩家捡起钥匙");
                _isPickUp = true;
                PickUpEffect(player);
                
                GetComponent<SpriteRenderer>().enabled = false; // 关闭渲染
            }
            // 钥匙打开门
            else if (_isPickUp && collision.TryGetComponent<Door>(out var door) && door)
            {
                Debug.Log("钥匙与门触碰");
                Debug.Log(SwitchableObjects);
                Debug.Log(door);
                // 匹配钥匙与门
                if (SwitchableObjects.Contains(door))
                {
                    door.OnSwitchOn();
                    Debug.Log("钥匙打开了门");
                }
            }
        }
    }
}