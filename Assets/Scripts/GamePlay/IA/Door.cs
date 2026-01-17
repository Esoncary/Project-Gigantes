using GamePlay.IA.Base;
using GamePlay.Pickups;
using UnityEngine;

namespace GamePlay.IA
{
    public class Door : Switchable
    {
        public override void OnSwitchOn()
        {
            Debug.Log("On Switch On");
            IsActive = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent<PlayerController>(out var player) && player)
            {
                if (IsActive)
                {
                    Debug.Log("门是开的");
                }
                else
                {
                    Debug.Log("门是关的");
                }
            }
        }
    }
}