using UnityEngine;

namespace GamePlay.Pickups
{
    public class Key : Switch, IPickUp
    {
        private bool _isPickUp;

        protected override void Start()
        {
            // 【重要修复】必须调用父类的 Start 以便执行 CheckStatus
            base.Start();
            if (!gameObject.activeSelf)
            {
                PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
                transform.SetParent(player.transform);
                transform.localPosition = Vector2.zero;
            }
        }

        // 处理“已经捡过”的情况
        protected override void HandleAlreadyInteracted()
        {
            // 如果存档已经记录捡过了，直接把物体关掉
            _isPickUp = true;
            gameObject.SetActive(false);
        }

        public void PickUpEffect(PlayerController player)
        {
            transform.SetParent(player.transform);
            transform.localPosition = Vector2.zero;
            GameDataMgr.Instance.RecordItem(itemID);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isPickUp && collision.TryGetComponent<PlayerController>(out var player))
            {
                _isPickUp = true;

                OnInteract();

                PickUpEffect(player);
                GetComponent<SpriteRenderer>().enabled = false;
                var col = GetComponent<Collider2D>();
                if (col) col.enabled = false;
            }
        }

        public void OnKeyUsed()
        {
            gameObject.SetActive(false);
        }
    }
}