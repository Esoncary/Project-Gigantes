using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : PlayerState
{
    //在死亡状态中，理应包含以下内容：广播死亡状态并且播放死亡动画，重新在复活点生成player
    
    public DieState(PlayerController _player, PlayerStateMachine _stateMachine, string _animName)
        : base(_player, _stateMachine, _animName) { }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
