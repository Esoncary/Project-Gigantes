using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strengthener : MonoBehaviour, IPickUp
{
    //这个是增强剂，它会短暂地将maxStorage和explosionStorage提高百分之20
    
    
    public float strenthenerTime;//强化持续时间

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
        player.strengthenerTimer = strenthenerTime;//启动强化计时器
        player.maxStorage += 35;//提高最大储量
        player.explosionStorageThrehold += 35;//提高爆炸储量阈值
        player.rb.velocity *= 1.5f;//拾取增强剂时瞬间提高50%的速度
        player.playerVFXController.PlayStrengthened();//播放增强剂特效
        Destroy(this.gameObject);//销毁自身
    }
}
