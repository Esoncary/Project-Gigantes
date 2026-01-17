using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMgr : MonoBehaviour
{
    #region 单例化
    public static LevelMgr Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("引用")]
    public PlayerController player; // 玩家的引用

    [Header("重生点坐标设置")]
    public Vector2 currentRebornPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //收听游戏事件
    private void OnEnable()
    {
        //收听玩家死亡事件，执行PlayerReborn函数
        GameEvents.PlayerDie += PlayerReborn;

        //收听重生点更新事件，执行UpdateRebornPoint函数
        GameEvents.UpdateRebornPoint += UpdateRebornPoint;
    }

    private void OnDisable()
    {
        // 停止收听：防止切换场景报错
        GameEvents.PlayerDie -= PlayerReborn;
        GameEvents.UpdateRebornPoint -= UpdateRebornPoint;
    }

    void PlayerReborn()
    {
        player.DieState.Reborn(currentRebornPos);

        //这里是音乐，关卡重置之类的逻辑，还没写
    }

    void UpdateRebornPoint(Vector2 pos)
    {
        currentRebornPos = pos;
    }
}
