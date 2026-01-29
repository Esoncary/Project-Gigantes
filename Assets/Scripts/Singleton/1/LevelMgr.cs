using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject playerPrefab;

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

    #region 监听广播函数
    //收听游戏事件
    private void OnEnable()
    {
        //收听玩家死亡事件，执行PlayerReborn，ReloadCurrentLevel函数
        //GameEvents.PlayerDie += PlayerReborn;暂时似乎不需要了，这种重生方式不适合游戏方式
        GameEvents.PlayerDie += StartReloadCurrentLevel;

        //收听重生点更新事件，执行UpdateRebornPoint函数
        GameEvents.UpdateRebornPoint += UpdateRebornPoint;
    }

    private void OnDisable()
    {
        // 停止收听：防止切换场景报错
        GameEvents.PlayerDie -= StartReloadCurrentLevel;
        GameEvents.UpdateRebornPoint -= UpdateRebornPoint;
    }
    #endregion

    #region 关卡载入函数和协程
    //切换关卡函数和协程
    public void StartLoadNextLevel(string levelName)
    {
        StartCoroutine(LoadNextLevel(levelName));//启动关卡切换协程


    }
    public IEnumerator LoadNextLevel(string levelName)
    {
        Debug.Log("开始载入下一个关卡");
        //保存所有持久化数据

        //异步加载新的场景
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);

        //黑屏淡入（UIManager中）
        // UIManager.Instance.StartBlackImageFadeIn();

        // 只要没加载完，就一直等待
        while (!operation.isDone)
        {
            yield return null;
        }

        //以下是新场景中的函数
        //加载所有持久化数据

        //设置新的重生点
        currentRebornPos = FindObjectOfType<LevelEntranceDoor>().transform.position;//将当前重生点设置为关卡入口门

        //生成player
        InstiatePlayer();
        //重新设定相机follow对象
        VirtualCameraController.Instance.ResetCameraTarget(player.transform);
        //黑屏淡出(使新场景显现)
        // UIManager.Instance.StartBlackImageFadeOut();
    }

    //重载当前关卡
    public void StartReloadCurrentLevel()
    {
        StartCoroutine(ReloadCurrentLevel());
    }
    private IEnumerator ReloadCurrentLevel()
    {
        Debug.Log("开始重载当前关卡");

        yield return StartCoroutine(UIMgr.Instance.BlackImageFadeIn()); // 等待淡入动画完成

        // 重新加载当前场景
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while (!op.isDone) yield return null; // 等待淡入动画完成

        yield return StartCoroutine(UIMgr.Instance.BlackImageFadeOut());
    }
    #endregion

    #region player重生或生成函数
    void PlayerReborn()
    {
        player.DieState.Reborn(currentRebornPos);

        //这里是音乐，关卡重置之类的逻辑，还没写
    }

    void UpdateRebornPoint(Vector2 pos)
    {
        currentRebornPos = pos;
    }

    public void InstiatePlayer()
    {
        // 1. 生成玩家实例
        GameObject newPlayerObj = Instantiate(playerPrefab, currentRebornPos, Quaternion.identity);

        // 2. 更新 LevelMgr 内部的 player 引用
        player = newPlayerObj.GetComponent<PlayerController>();
    }

    #endregion
}
