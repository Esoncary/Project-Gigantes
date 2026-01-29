using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMgr : MonoBehaviour
{
    public enum GameState { MainMenu, Playing, Paused }
    // 1. 定义静态实例
    public static GameMgr Instance { get; private set; }

    public GameState CurrentState;

    private void Awake()
    {
        // 2. 单例保护逻辑：确保场景中只有一个管理器
        if (Instance == null)
        {
            Instance = this;
            // 选做：如果是全局管理器，通常不随场景销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 更新游戏状态
    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        switch (newState)
        {
            case GameState.Paused:
                Time.timeScale = 0f; // 停止物理和计时
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
        }
        Debug.Log("Game State Changed to: " + newState);
    }

    //玩家死亡或者手动重启当前关卡
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        ChangeGameState(GameState.Playing);
    }

    

}
