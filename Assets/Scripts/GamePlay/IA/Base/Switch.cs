using System.Collections.Generic;
using GamePlay.Player.Interface;
using UnityEngine;

namespace GamePlay.IA.Base
{
    public class Switch : MonoBehaviour
    {
        [Header("开关行为")] [SerializeField] private bool initialIsOn;

        public bool IsOn { get; set; }
        protected HashSet<ISwitchable> SwitchableObjects { get; set; }
        private bool _isLeave = true; // 玩家是否在在开关区域外
        
        private void Awake()
        {
            // 初始化开关状态
            IsOn = initialIsOn;
        }

        private void Start()
        {
            // 通知所有注册的对象当前状态
            Notify();
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
        
        // 将可切换对象注册到自身
        public void RegisterSwitchable(ISwitchable switchable)
        {
            SwitchableObjects ??= new HashSet<ISwitchable>();
            SwitchableObjects.Add(switchable);
        }

        // 切换开关状态
        void Toggle()
        {
            IsOn = !IsOn;
            Notify();
        }

        private void Notify()
        {
            foreach (var obj in SwitchableObjects)
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