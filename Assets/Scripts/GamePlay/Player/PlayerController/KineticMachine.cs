using UnityEngine;

public class KineticMachine : MonoBehaviour
{
    //提前说明currentStorageFreezeTimer和targetStorageFreezeTimer值的意义，它们有三种值，对应三态：-1意味着计时器被关闭，<=0 意味着计时器跑到了0但是还没有被关闭，>0意味着计时器正在运作中。如果不将前两个值故意区分开来，在计时器跑完且冻结前提满足的情况下（比如当前储量大于目标储量），计时器会他妈地再次启动从而当前储量永远不会下降。我被这个bug搞了至少两个小时，非常难受。

    private PlayerController player;
    [SerializeField]

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Debug.Log("k is working");
        if (player.coolerTimer <= 0 && player.kineticMachineRecoverFromReleaseTimer <= 0)//前者是冷却剂，后者是装置冷却时间，整个装置在释放后也会冷却不运作
        {
            KineticMachineOperate();
            UIManager.Instance.GetPanel<GamePanel>()?.ChangeStorageUI(true);
        }

        if (player.strengthenerTimer <= 0 && player.strengthenerTimer != -1)
        {
            player.maxStorage = 60;//恢复最大储量
            player.explosionStorageThrehold = 40;//恢复爆炸储量阈值
            player.strengthenerTimer = -1;//关闭强化计时器
        }



    }
    void KineticMachineOperate()//动力装置的所有运作逻辑
    {
        player.currentVelocityMag = player.rb.velocity.magnitude;

        //先来处理目标储量
        //如果速度模大于等于目标储量，则将速度模赋值给目标储量
        if (player.targetStorage <= player.currentVelocityMag)
        {
            // if (player.targetStorage != 0)
            // Debug.Log("targetStorage:" + player.targetStorage);
            player.targetStorageFreezeTimer = -1;//如果有冻结目标储量计时器正在运作，关掉它
            SetTargetStorage();//这是一个赋值函数
            // Debug.Log("A");
        }
        //如果速度模小于目标储量
        else if (player.targetStorage > player.currentVelocityMag)
        {
            if (player.targetStorageFreezeTimer == -1)//如果此时计时器还没有启动
            {
                player.targetStorageFreezeTimer = player.targetStorageFreezeTime;//启动冻结目标储量计时
                // Debug.Log("C");
            }
            else if (player.targetStorageFreezeTimer <= 0 && player.targetStorageFreezeTimer != -1)//如果计时器跑完了，且它还没有被关掉
            {
                player.targetStorageFreezeTimer = -1;//关掉计时器
                SetTargetStorage();//将速度模赋值给目标储量,也就是允许目标储量下降
                // Debug.Log("=B");
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
                currentStorageDecrease();//当前储量下降函数
            }
        }

        //如果当前储量冻结计时器小于等于0且当前储量冻结计时器不处于-1状态，则允许当前储量下降
        if (player.currentStorageFreezeTimer <= 0 && player.currentStorageFreezeTimer != -1)
        {

            currentStorageDecrease();//当前储量下降函数
        }

        //现在来处理过载状态
        if (player.currentStorage >= player.explosionStorageThrehold)
        {
            if (player.explosionTimer == -1)//如果此时计时器还没有启动
            {
                player.isOverloaded = true;//将过载状态设为真
                player.explosionTimer = player.explosionTime;//启动过载计时器
                player.playerVFXController.PlayeOverload();//启动过载特效
                SoundEffectMgr.Instance.PlaySound("overload/overload_enter", true);
            }
            Overloaded();//过载行为函数
        }
        else if (player.currentStorage < player.explosionStorageThrehold)
        {
            player.isOverloaded = false;//将过载状态设为假
            player.explosionTimer = -1;//关闭过载计时器
            player.playerVFXController.StopOverload();//关闭过载特效
            SoundEffectMgr.Instance.StopSound("overload/overload_enter");
        }
    }


    // 目标储量设定函数
    public void SetTargetStorage()
    {
        if (player.rb.velocity.y < -0.1f)
        {
            player.targetStorage = player.currentVelocityMag * player.Kin_gravityContractionScale;
        }
        else
        {
            player.targetStorage = player.currentVelocityMag * 1.1f;//在数值调试中有些问题，加了个1.1f来弥补
        }
    }
    // public void SetTargetStorage(float n)
    // {
    //     player.targetStorage = n;
    // }
    //当前储量上升速度计算函数
    void currentStorageIncreaseSpeedCalculate()
    {
        //当前储量上升速度与目标储量成正比，f是我随意设定的系数
        player.currentStorageIncreaseSpeed = player.currentStorageDefaultIncreaseSpeed * (player.targetStorage / 5f);
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
        if (player.explosionTimer <= 0 && player.explosionTimer != -1 && player.StateMachine.CurrentState != player.DieState)//如果计时器结束且不已经处于死亡状态
        {

            player.StateMachine.ChangeState(player.DieState);
            Debug.Log("玩家死了");
            player.explosionTimer = -1;//关闭计时器
        }
    }
}