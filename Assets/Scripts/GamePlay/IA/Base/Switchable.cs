using System.Collections.Generic;
using GamePlay.Player.Interface;
using UnityEngine;

namespace GamePlay.IA.Base
{
    public abstract class Switchable : MonoBehaviour, ISwitchable
    {
        [Header("开关绑定")]
        [Tooltip("绑定开关对象")]
        [SerializeField] protected List<Switch> boundSwitches = new List<Switch>();
        [Tooltip("是否默认激活")]
        [SerializeField] protected bool defaultIsOn = false;//true

        public bool IsActive { get; protected set; }

        protected virtual void Awake()
        {
            // if (boundSwitch != null)
            if (boundSwitches != null && boundSwitches.Count > 0)
            {
                IsActive = false;
                // 遍历所有绑定的开关并进行注册
                foreach (var sw in boundSwitches)
                {
                    if (sw != null)
                    {
                        sw.RegisterSwitchable(this);
                    }
                }
            }
            else
            {
                IsActive = defaultIsOn;
            }
        }

        public virtual void OnSwitchOn() => IsActive = true;
        public virtual void OnSwitchOff() => IsActive = false;
        // 【新增】辅助方法：获取绑定的开关数量
        public int GetBoundSwitchCount()
        {
            return boundSwitches != null ? boundSwitches.Count : 0;
        }
    }
}