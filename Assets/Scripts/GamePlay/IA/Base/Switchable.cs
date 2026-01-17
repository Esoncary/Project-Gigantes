using GamePlay.Player.Interface;
using UnityEngine;

namespace GamePlay.IA.Base
{
    public abstract class Switchable : MonoBehaviour, ISwitchable
    {
        [Header("开关绑定")]
        [Tooltip("绑定开关对象")]
        [SerializeField] protected Switch boundSwitch;
        [Tooltip("是否默认激活")]
        [SerializeField] protected bool defaultIsOn = true;

        public bool IsActive { get; protected set; }

        protected virtual void Awake()
        {
            if (boundSwitch != null)
            {
                IsActive = boundSwitch.IsOn;
                boundSwitch.RegisterSwitchable(this); // 绑定开关
            }
            else
            {
                IsActive = defaultIsOn;
            }
        }

        public virtual void OnSwitchOn() => IsActive = true;
        public virtual void OnSwitchOff() => IsActive = false;
    }
}