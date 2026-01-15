using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebornLandmark : MonoBehaviour, ILandmark
{
    public bool isActivated = false;//地标是否处于激活状态

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ILandmark.LandmarkEffect()//广播更新事件，通知LevelMgr更新当前重生点和其它重生点关闭激活状态
    {
        GameEvents.BroadcastUpdateRebornPoint(transform.position);
    }


    private void OnEnable()
    {
        GameEvents.UpdateRebornPoint += UpdateRebornLandmarkState;
    }

    private void OnDisable()
    {
        GameEvents.UpdateRebornPoint -= UpdateRebornLandmarkState;
    }


    
    void UpdateRebornLandmarkState(Vector2 pos)//更新地标激活状态。更新重生pos已经在levelmgr中完成，这里主要用于更新视觉效果
    {
        if ((Vector2)this.transform.position == pos)//如果玩家触碰的地标就是当前地标
        {
            if (isActivated)
            {
                return;//已经是激活状态，直接返回
            }
            else
            {
                isActivated = true;//设置为激活状态
                //在这里可以添加一些激活时的视觉效果，比如改变颜色或播放动画
            }
        }
        else//如果玩家触碰的不是当前地标
        {
            if (isActivated)
            {
                isActivated = false;//关闭激活状态
                
            }
            else
            {
                return;//已经是不激活状态，直接返回
            }
        }
    }
}
