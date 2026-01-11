using GamePlay.Player.Interface;
using UnityEngine;

namespace GamePlay.IA
{
    public abstract class Switchable : MonoBehaviour, ISwitchable
    {
        [Header("开关绑定")]
        [Tooltip("绑定开关对象")]
        [SerializeField] protected Switch boundSwitch;
        [Tooltip("是否跟随开关状态")]
        [SerializeField] protected bool followSwitch;
        [Tooltip("是否默认激活")]
        [SerializeField] protected bool defaultIsOn = true;

        public bool IsActive { get; protected set; }

        protected virtual void Awake()
        {
            if (followSwitch && boundSwitch != null)
            {
                IsActive = boundSwitch.IsOn;
                boundSwitch.RegisterSwitchable(this);
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