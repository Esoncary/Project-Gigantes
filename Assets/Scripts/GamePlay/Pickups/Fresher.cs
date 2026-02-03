using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fresher : MonoBehaviour, IPickUp
{
    //这个是刷新剂，它会刷新release的冷却和动力装置的运作冷却
    
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
        player.releaseCoolingTimer = -1f;//刷新release冷却
        player.kineticMachineRecoverFromReleaseTimer = -1f;//刷新动力装置运作冷却
        Destroy(this.gameObject);//销毁自身
    }
}
