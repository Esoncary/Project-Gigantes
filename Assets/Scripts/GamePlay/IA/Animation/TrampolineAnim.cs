using UnityEngine;

namespace GamePlay.IA.Animation
{
    /// <summary>
    /// 蹦床动画控制器，负责播放压缩回弹动画
    /// </summary>
    public class TrampolineAnim : MonoBehaviour
    {
        [Header("动画设置")]
        [Tooltip("压缩动画持续时间（秒），从第一帧到压缩到底部的时间")]
        [SerializeField] private float compressionDuration = 0.5f;

        [Tooltip("常规动画播放速度倍率")]
        [SerializeField] private float animSpeed = 1f;

        [Header("调试")]
        [SerializeField] private bool debugMode = false;
        
        private Animator _animator;
        private PlayerController player;
        private static readonly int CompressionTrigger = Animator.StringToHash("Compression");
        private bool _isPlayingCompression = false;
        private bool _isPaused = false;

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
            if (_animator != null)
            {
                _animator.speed = animSpeed;
            }
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
            if (_animator != null)
            {
                _isPlayingCompression = true;
                _isPaused = false;
                // 原始动画时长为1秒，通过调整speed实现目标时长
                // speed = 原始时长 / 目标时长
                _animator.speed = 1f / compressionDuration;
                _animator.SetTrigger(CompressionTrigger);
                if (debugMode)
                    Debug.Log($"[TrampolineAnim] 播放压缩动画，时长: {compressionDuration}秒");
            }
        }

        /// <summary>
        /// 暂停动画播放
        /// </summary>
        public void Pause()
        {
            if (_animator != null && !_isPaused)
            {
                _isPaused = true;
                _animator.speed = 0f;
                if (debugMode)
                    Debug.Log("[TrampolineAnim] 动画已暂停");
            }
        }

        /// <summary>
        /// 恢复动画播放
        /// </summary>
        public void Resume()
        {
            if (_animator != null && _isPaused)
            {
                _isPaused = false;
                // 根据当前状态恢复速度
                _animator.speed = _isPlayingCompression ? 1f / compressionDuration : animSpeed;
                if (debugMode)
                    Debug.Log("[TrampolineAnim] 动画已恢复");
            }
        }

        /// <summary>
        /// 设置动画播放速度
        /// </summary>
        public void SetAnimSpeed(float speed)
        {
            if (_animator != null)
            {
                _animator.speed = speed;
            }
        }

        /// <summary>
        /// 重置动画状态
        /// </summary>
        public void ResetAnim()
        {
            if (_animator != null)
            {
                _animator.ResetTrigger(CompressionTrigger);
                _animator.Play("Idle", 0, 0f);
            }
        }
    }
}