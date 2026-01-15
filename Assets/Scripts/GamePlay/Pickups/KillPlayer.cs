using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour, IPickUp
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IPickUp.PickUpEffect(PlayerController player)
    {
        Debug.Log("成功杀死玩家");
        player.StateMachine.ChangeState(player.DieState);
    }
}
