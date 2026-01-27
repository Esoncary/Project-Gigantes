// using System.Collections.Generic;
// using GamePlay.Player.Interface;
// using UnityEngine;

// namespace GamePlay.IA.Base
// {
//     public class Switch : MonoBehaviour
//     {
//         [Header("开关行为")][SerializeField] private bool initialIsOn;

//         public bool IsOn { get; set; }
//         protected HashSet<ISwitchable> SwitchableObjects { get; set; }
//         private bool _isLeave = true; // 玩家是否在在开关区域外

//         private void Awake()
//         {
//             // 初始化开关状态
//             IsOn = initialIsOn;
//         }

//         private void Start()
//         {
//             // 通知所有注册的对象当前状态
//             Notify();
//         }

//         private void OnTriggerEnter2D(Collider2D collision)
//         {
//             if (_isLeave)
//             {
//                 Debug.Log("开关被触发");
//                 _isLeave = false;
//                 Toggle();
//             }
//         }

//         private void OnTriggerExit2D(Collider2D collision)
//         {
//             _isLeave = true;
//         }

//         // 将可切换对象注册到自身
//         public void RegisterSwitchable(ISwitchable switchable)
//         {
//             SwitchableObjects ??= new HashSet<ISwitchable>();
//             SwitchableObjects.Add(switchable);
//         }

//         // 切换开关状态
//         void Toggle()
//         {
//             IsOn = !IsOn;
//             Notify();
//         }

//         private void Notify()
//         {
//             if (SwitchableObjects == null) return;

//             foreach (var obj in SwitchableObjects)
//             {
//                 if (IsOn)
//                 {
//                     obj.OnSwitchOn();
//                 }
//                 else
//                 {
//                     obj.OnSwitchOff();
//                 }
//             }
//         }
//     }
// }
using System.Collections.Generic;
using GamePlay.Player.Interface;
using UnityEngine;

namespace GamePlay.IA.Base
{
    public class Switch : MonoBehaviour
    {
        [Header("开关行为")]
        [SerializeField] private bool initialIsOn;

        public bool IsOn { get; protected set; } // 修改为 protected set，防止外部随意修改，但子类可以改
        protected HashSet<ISwitchable> SwitchableObjects { get; set; }
        private bool _isLeave = true;

        protected virtual void Awake() // 改为 virtual 方便子类重写
        {
            IsOn = initialIsOn;
        }

        protected virtual void Start()
        {
            // 【修改点】不要在 Start 里无条件 Notify，
            // 否则 Key 一绑定就会导致 Door 计数 +1
            if (IsOn)
            {
                Notify();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // 只有作为普通物理开关（如踏板）时才在这里触发
            // Key 有自己的触发逻辑，会在 Key.cs 里重写或处理
            if (_isLeave && !GetComponent<GamePlay.Pickups.Key>())
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

        public void RegisterSwitchable(ISwitchable switchable)
        {
            SwitchableObjects ??= new HashSet<ISwitchable>();
            SwitchableObjects.Add(switchable);
        }

        public void Toggle()
        {
            SetState(!IsOn); // 使用统一的方法设置状态
        }

        // 【新增】供外部或子类明确设置状态的方法
        public void SetState(bool state)
        {
            IsOn = state;
            Notify();
        }

        protected void Notify()
        {
            if (SwitchableObjects == null) return;

            // 复制一份列表进行遍历，防止遍历过程中集合被修改导致报错
            foreach (var obj in new List<ISwitchable>(SwitchableObjects))
            {
                if (obj == null) continue; // 安全检查

                if (IsOn) obj.OnSwitchOn();
                else obj.OnSwitchOff();
            }
        }
    }
}