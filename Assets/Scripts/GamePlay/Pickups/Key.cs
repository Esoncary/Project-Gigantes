using UnityEngine;
using System.Collections;

namespace GamePlay.Pickups
{
    public class Key : Switch, IPickUp
    {
        [Header("道具共有参数设置")]//用于钥匙浮动效果
        public float floatRange = 0.2f;
        public float floatSpeed = 2.0f;
        protected Vector3 startPos;



        private bool _isPickUp;
        private PlayerController _cachedPlayer; // 缓存Player引用
        private bool _isBindingToPlayer; // 防止重复协程

        // 优先初始化itemID + 启动协程等待Player
        protected override void Awake()
        {
            base.Awake(); // 调用LevelItem的Awake生成唯一itemID
            StartCoroutine(WaitForPlayerThenBindKey()); // 协程等待Player加载
            startPos = transform.position;
        }

        protected override void Start()
        {
            base.Start(); // 执行Switch→LevelItem的CheckStatus逻辑

            // 如果已标记为拾取，直接隐藏视觉/碰撞（不影响挂载逻辑）
            if (_isPickUp)
            {
                DisableKeyVisualAndCollider();
            }
        }

        private void Update()
        {
            //钥匙浮动逻辑
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatRange;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }

        // 核心逻辑：协程等待Player加载完成，再处理存档钥匙的挂载
        private IEnumerator WaitForPlayerThenBindKey()
        {
            // 防止重复执行协程
            if (_isBindingToPlayer) yield break;
            _isBindingToPlayer = true;

            // 循环等待直到找到Player（最多等5秒，避免死等）
            float waitTime = 0;
            while (_cachedPlayer == null && waitTime < 5f)
            {
                var playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    _cachedPlayer = playerObj.GetComponent<PlayerController>();
                }
                yield return new WaitForEndOfFrame();
                waitTime += Time.deltaTime;
            }

            // 超时未找到Player的容错
            if (_cachedPlayer == null)
            {
                Debug.LogError($"[{gameObject.name}] 等待5秒仍未找到Player，钥匙挂载失败！");
                _isBindingToPlayer = false;
                yield break;
            }

            // 如果存档中存在该钥匙，直接挂载到Player下
            if (_isPickUp)
            {
                BindKeyToPlayer();
            }

            _isBindingToPlayer = false;
        }

        // 重写存档处理逻辑：标记已拾取 + 触发后续挂载
        protected override void HandleAlreadyInteracted()
        {
            _isPickUp = true;
            Debug.Log($"[{gameObject.name}] 存档检测到已拾取，准备挂载到Player");

            // 隐藏视觉和碰撞（先执行，避免玩家看到存档钥匙）
            DisableKeyVisualAndCollider();

            // 如果Player已缓存，直接挂载；否则等协程处理
            if (_cachedPlayer != null)
            {
                BindKeyToPlayer();
            }
            // 否则协程会在找到Player后自动挂载
        }

        // 核心方法：将钥匙挂载到Player下（统一逻辑）
        private void BindKeyToPlayer()
        {
            // 防止重复挂载
            if (transform.parent == _cachedPlayer.transform) return;

            // 挂载到Player下 + 重置本地位置
            transform.SetParent(_cachedPlayer.transform, false); // false：不保留世界坐标
            transform.localPosition = Vector2.zero; // 可根据需求调整位置（比如设为(0.5f, 0)）
            transform.localRotation = Quaternion.identity; // 重置旋转

            Debug.Log($"[{gameObject.name}] 已成功挂载到Player：{_cachedPlayer.gameObject.name}");
        }

        // 拾取效果（主动拾取时的逻辑，保持不变）
        public void PickUpEffect(PlayerController player)
        {
            if (player == null) return;

            // 主动拾取时也挂载到Player下
            BindKeyToPlayer();

            // 记录存档（同步到持久化数据）
            GameDataMgr.Instance.RecordItem(itemID);
            if (!GameDataMgr.Instance.currentSave.suspendData.interactedItems.Contains(itemID))
            {
                GameDataMgr.Instance.currentSave.suspendData.interactedItems.Add(itemID);
                GameDataMgr.Instance.SavePlayerSaveData();
            }
        }

        // 触发拾取的逻辑（保持不变）
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isPickUp || !collision.CompareTag("Player") || _cachedPlayer == null) return;

            _isPickUp = true;
            OnInteract();
            PickUpEffect(_cachedPlayer);
            DisableKeyVisualAndCollider();
        }

        // 辅助方法：隐藏钥匙的视觉和碰撞（复用）
        private void DisableKeyVisualAndCollider()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.enabled = false;

            var collider2D = GetComponent<Collider2D>();
            if (collider2D != null) collider2D.enabled = false;
        }

        public void OnKeyUsed()
        {
            gameObject.SetActive(false);
        }
    }
}