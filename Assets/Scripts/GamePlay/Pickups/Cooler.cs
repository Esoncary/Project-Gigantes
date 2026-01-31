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
        player.playerVFXController.PlayCooled();//启动冷冻特效。这种启动方式有问题，但是先这样吧
        player.playerVFXController.StopOverload();//关闭过载特效。如果角色处于过载状态吃到，关闭过载特效
        Destroy(this.gameObject);//销毁自身

    }
}
