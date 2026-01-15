using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooler : MonoBehaviour, IPickUp
{
    [Header("coller参数设置")]
    public float coolTime;

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
        player.coolerTimer = coolTime;//启动冷冻计时器
        Destroy(this.gameObject);//销毁自身

    }
}
