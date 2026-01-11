namespace GamePlay.IA
{
    public class Switch : MonoBehaviour
    {
        [Header("开关行为")] [SerializeField] private bool initialIsOn;

        private readonly List<ISwitchable> _switchableObjects = new();
        private bool _isLeave = true; // 玩家是否在在开关区域外

        public bool IsOn { get; private set; }

        private void Awake()
        {
            // 初始化开关状态
            IsOn = initialIsOn;
        }

        private void Start()
        {
            // 通知所有注册的对象当前状态
            NotifyAll();
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isLeave)
            {
                Debug.Log("开关被触发");
                _isLeave = false;
                Toggle();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _isLeave = true;
        }

        // 切换开关状态
        void Toggle()
        {
            IsOn = !IsOn;
            NotifyAll();
        }
        
        // 注册可切换对象
        public void RegisterSwitchable(ISwitchable switchable)
        {
            if (switchable != null && !_switchableObjects.Contains(switchable))
            {
                _switchableObjects.Add(switchable);
            }
        }


        private void NotifyAll()
        {
            foreach (var obj in _switchableObjects)
            {
                if (IsOn)
                {
                    obj.OnSwitchOn();
                }
                else
                {
                    obj.OnSwitchOff();
                }
            }
        }
    }
}