using System.Collections;
using System.Threading.Tasks;
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
        private Animator _animator;
        private static readonly int DoorOpenTrigger = Animator.StringToHash("Open");

        protected override void Awake()
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("[TrampolineAnim] 未找到Animator组件！", this);
            }
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
                _animator.SetTrigger(DoorOpenTrigger);
                SoundEffectMgr.Instance.PlaySound("door/door-open");
            }
        }

        private async Task OnTriggerEnter2D(Collider2D collision)
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
                    await ShowGameOverPanelAfterDelay(1f);
                }
                else
                {
                    int total = (boundSwitches != null && boundSwitches.Count > 0) ? boundSwitches.Count : manualRequiredKeys;
                    Debug.Log($"门锁着 (进度: {_currentActiveCount} / {total})");
                }
            }
        }
        private async Task ShowGameOverPanelAfterDelay(float delay)
        {
            await Task.Delay((int)(delay * 1000));

            await SceneMgr.Instance.SceneTransitionAsync(async () =>
            {
                GameDataMgr.Instance.ClearSuspendData();
                UIManager.Instance.HidePanel<GamePanel>();
                //260215：进入scenepanel后销毁玩家
                if (SceneMgr.Instance.playerObj != null)
                {
                    GameObject.Destroy(SceneMgr.Instance.playerObj);
                }
                UIManager.Instance.ShowPanel<ScenePanel>();
                // Debug.Log("游戏结束" + UIManager.Instance.GetPanel<ScenePanel>().selectedLevelIndex);
                ScenePanel a = UIManager.Instance.GetPanel<ScenePanel>();
                a.SelectNextLevel();
            });

        }
    }
}