using UnityEngine;

public class KineticMachine : MonoBehaviour
{
    //提前说明currentStorageFreezeTimer和targetStorageFreezeTimer值的意义，它们有三种值，对应三态：-1意味着计时器被关闭，<=0 意味着计时器跑到了0但是还没有被关闭，>0意味着计时器正在运作中。如果不将前两个值故意区分开来，在计时器跑完且冻结前提满足的情况下（比如当前储量大于目标储量），计时器会他妈地再次启动从而当前储量永远不会下降。我被这个bug搞了至少两个小时，非常难受。

    private PlayerController player;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        //每帧同步currentVelocityMag
        player.currentVelocityMag = player.rb.velocity.magnitude;

        if (player.coolerTimer <= 0)
        {
            KineticMachineOperate();
        }

        

        
    }
    void KineticMachineOperate()//动力装置的所有运作逻辑
    {
        //先来处理目标储量
        //如果速度模大于等于目标储量，则将速度模赋值给目标储量
        if (player.targetStorage <= player.currentVelocityMag)
        {
            player.targetStorageFreezeTimer = -1;//如果有冻结目标储量计时器正在运作，关掉它
            SetTargetStorage();//这是一个赋值函数
        }
        //如果速度模小于目标储量
        else if (player.targetStorage > player.currentVelocityMag)
        {
            if (player.targetStorageFreezeTimer == -1)//如果此时计时器还没有启动
            {
                player.targetStorageFreezeTimer = player.targetStorageFreezeTime;//启动冻结目标储量计时
            }
            else if (player.targetStorageFreezeTimer <= 0 && player.targetStorageFreezeTimer != -1)//如果计时器跑完了，且它还没有被关掉
            {
                player.targetStorageFreezeTimer = -1;//关掉计时器
                SetTargetStorage();//将速度模赋值给目标储量,也就是允许目标储量下降
            }

        }

        //现在来处理当前储量
        //计算当前储量上升速度
        currentStorageIncreaseSpeedCalculate();
        //如果当前储量小于等于目标储量，则当前储量上升
        if (player.currentStorage <= player.targetStorage)
        {
            player.currentStorageFreezeTimer = -1;//如果有当前储量冻结计时器正在运作，关掉它
            CurrentStorageIncrease();//当前储量上升函数
        }
        //如果当前储量大于目标储量
        else if (player.currentStorage > player.targetStorage)
        {
            if (player.currentStorageFreezeTimer == -1)//如果计时器是关闭状态
            {
                player.currentStorageFreezeTimer = player.currentStorageFreezeTime;//启动冻结当前储量计时器
            }
            else if (player.currentStorageFreezeTimer <= 0 && player.currentStorageFreezeTimer != -1)//如果计时器跑完了，且它还没有被关掉 
            {
                player.currentStorageFreezeTimer = -1;//关掉计时器
                currentStorageDecrease();//当前储量下降函数
            }
        }

        //如果当前储量冻结计时器小于等于0且当前储量冻结计时器不处于-1状态，则允许当前储量下降
        if (player.currentStorageFreezeTimer <= 0 && player.currentStorageFreezeTimer != -1)
        {
            player.currentStorageFreezeTimer = -1;//关闭当前储量冻结计时器
            currentStorageDecrease();//当前储量下降函数
        }

        //现在来处理过载状态
        if (player.currentStorage >= player.maxStorage)
        {
            player.isOverloaded = true;//将过载状态设为真
            player.explosionTimer = player.explosionTime;//启动过载计时器
            Overloaded();//过载行为函数
        }
        else if (player.currentStorage < player.maxStorage)
        {
            player.isOverloaded = false;//将过载状态设为假
            player.explosionTimer = -1;//关闭过载计时器
        }
    }


    // 目标储量设定函数
    void SetTargetStorage()
    {
        player.targetStorage = player.currentVelocityMag;
    }

    //当前储量上升速度计算函数
    void currentStorageIncreaseSpeedCalculate()
    {
        //当前储量上升速度与目标储量成正比，20f是我设定的系数
        player.currentStorageIncreaseSpeed = player.currentStorageDefaultIncreaseSpeed * (player.targetStorage / 20f);
    }

    //当前储量上升函数
    void CurrentStorageIncrease()
    {
        player.currentStorage = Mathf.MoveTowards(player.currentStorage, player.targetStorage, player.currentStorageIncreaseSpeed * Time.deltaTime);
    }

    //当前储量下降函数
    void currentStorageDecrease()
    {
        player.currentStorage = Mathf.MoveTowards(player.currentStorage, player.targetStorage, player.currentStorageDecreaseSpeed * Time.deltaTime); 
    }

    

    // 过载/爆炸函数
    private void Overloaded()
    {
        //过载时没有特殊行为，但是如果玩家在过载时release，动力会更强，这条逻辑会写在release中

        //处理爆炸计时器
        if (player.explosionTimer <= 0)//如果计时器结束
        {
            //player.StateMachine.ChangeState(player.DieState)// 切换到死亡状态，但是死亡状态还没写
             
                Debug.LogError("能量过载爆炸！");
        }
       
        
    }
}