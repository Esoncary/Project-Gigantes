using UnityEngine;

namespace GamePlay.IA.Animation
{
    /// <summary>
    /// 蹦床动画控制器，负责播放压缩回弹动画
    /// </summary>
    public class TrampolineAnim : MonoBehaviour
    {
        [Header("调试")]
        [SerializeField] private bool debugMode = false;
        
        private Animator _animator;
        private PlayerController player;
        private static readonly int CompressionTrigger = Animator.StringToHash("Bounce");
        private bool _isPlayingCompression = false;
        

        public TrampolineAnim(PlayerController _player)
        {
            player = _player;
        }
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogError("[TrampolineAnim] 未找到Animator组件！", this);
            }
        }

        private void Start()
        {
        }
        
        // 玩家实体触碰蹦床
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("蹦床开始形变 time: " + Time.timeAsDouble);
            PlayCompression();
        }

        /// <summary>
        /// 播放压缩动画（供外部调用）
        /// </summary>
        private void PlayCompression()
        {
            // 获取当前动画状态信息
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

            // 如果正在播放 compression 动画，或正在过渡到 compression，则忽略
            if (stateInfo.IsName("compression") || _animator.IsInTransition(0))
            {
                if (debugMode)
                    Debug.Log("[TrampolineAnim] 动画播放中，忽略触发");
                return;
            }
            _animator.SetTrigger(CompressionTrigger);
        }
    }
}