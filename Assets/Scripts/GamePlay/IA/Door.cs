using GamePlay.IA.Base;
using GamePlay.Pickups; // 引用 Key 所在的命名空间
using UnityEngine;

namespace GamePlay.IA
{
    public class Door : Switchable
    {
        [Header("Door Settings")]
        public int manualRequiredKeys = 0;
        private int _currentActiveCount = 0;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void OnSwitchOn() => CheckAllSwitches();
        public override void OnSwitchOff() => CheckAllSwitches();

        private void CheckAllSwitches()
        {
            if (boundSwitches != null && boundSwitches.Count > 0)
            {
                int activeCount = 0;
                foreach (var sw in boundSwitches)
                {
                    // 只要开关不为空且是开启状态
                    if (sw != null && sw.IsOn) activeCount++;
                }

                _currentActiveCount = activeCount;

                if (activeCount >= boundSwitches.Count) OpenDoor();
                else IsActive = false;
            }
            else
            {
                // 手动计数模式（如果不使用绑定列表）
                if (_currentActiveCount >= manualRequiredKeys) OpenDoor();
            }
        }

        private void OpenDoor()
        {
            if (!IsActive)
            {
                Debug.Log("【门】条件满足，开门！");
                IsActive = true;
                // 这里播放动画等
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 检测是否是玩家
            if (collision.TryGetComponent<PlayerController>(out var player))
            {
                // 1. 【核心修改】门主动搜寻玩家身上的钥匙
                // 获取玩家身上所有的 Key 组件
                var keysOnPlayer = player.GetComponentsInChildren<Key>();

                foreach (var key in keysOnPlayer)
                {
                    // 如果这把钥匙在门的“白名单”里，并且还没被使用
                    if (boundSwitches.Contains(key) && !key.IsOn)
                    {
                        Debug.Log($"【门】发现匹配的钥匙: {key.name}");

                        // 激活钥匙（Switch变为On）
                        key.SetState(true);

                        // 让钥匙执行“被使用”的视觉效果（隐藏自己）
                        key.OnKeyUsed();
                    }
                }

                // 2. 搜寻完后，立即检查门的状态
                CheckAllSwitches();

                // 3. 输出提示信息
                if (IsActive)
                {
                    Debug.Log("门是开的，请进");
                    UIManager.Instance.ShowPanel<GameOverPanel>();
                }
                else
                {
                    int total = (boundSwitches != null && boundSwitches.Count > 0) ? boundSwitches.Count : manualRequiredKeys;
                    Debug.Log($"门锁着 (进度: {_currentActiveCount} / {total})");
                }
            }
        }
    }
}