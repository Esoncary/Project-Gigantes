using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitDoor : MonoBehaviour,IInteractable
{
    public string levelName;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IInteractable.Interact()
    {
        LevelMgr.Instance.StartLoadNextLevel(levelName);//玩家交互后，跳转到下一个关卡
    }
}
