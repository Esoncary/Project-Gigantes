using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraController : MonoBehaviour
{
    #region 单例化
    public static VirtualCameraController Instance { get; private set; }

    private CinemachineConfiner2D confiner;
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

        //获取组件引用
        vcam = GetComponent<CinemachineVirtualCamera>();
    }
    #endregion

    CinemachineVirtualCamera vcam;

    // 过场动画相关字段
    private float originalOrthoSize;
    private Coroutine cinematicCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (confiner != null && confiner.m_BoundingShape2D == null)
        {
            ResetCameraBound();//绑定相机边界
        }

    }

    public void ResetCameraTarget(Transform playerTransform)
    {
        // Debug.Log("state: ");
        if (vcam != null)
        {
            vcam.Follow = playerTransform;
            // 2. 强制提升优先级，确保新场景的 Brain 能立刻切换到这个相机
            // 先降后升，触发内部刷新
            vcam.Priority = 0;
            vcam.Priority = 10;

            // vcam.LookAt = playerTransform; // 2D 游戏通常只需要 Follow，不需要 LookAt
            Debug.Log("相机已成功绑定新角色！");
        }
    }

    public void ResetCameraBound()//重新寻找相机边界
    {
        GameObject cameraBound = GameObject.Find("CameraBound");
        Collider2D cameraBoundCol;
        if (cameraBound != null)
        {
            cameraBoundCol = cameraBound.GetComponent<Collider2D>();
            if (cameraBoundCol != null)
            {
                confiner.m_BoundingShape2D = cameraBoundCol;
                confiner.InvalidateCache();
            }
        }

    }

    #region 过场动画功能

    /// <summary>
    /// 启动过场模式：摄像头移动到指定位置并缩放
    /// </summary>
    /// <param name="targetPosition">摄像头目标位置（世界坐标 X, Y, Z）</param>
    /// <param name="orthoSize">摄像头缩放大小</param>
    /// <param name="duration">持续时间（秒）</param>
    /// <param name="startDuration">移动至目标的时间</param>
    /// <param name="endDuration">从目标返回的时间</param>
    public void StartCinematic(Vector3 targetPosition, float orthoSize, float duration, float startDuration, float endDuration)
    {
        if (cinematicCoroutine != null)
        {
            StopCoroutine(cinematicCoroutine);
        }
        cinematicCoroutine = StartCoroutine(CinematicCoroutine(targetPosition, orthoSize, duration, startDuration, endDuration));
    }

    private IEnumerator CinematicCoroutine(Vector3 targetPosition, float orthoSize, float duration, float startDuration, float endDuration)
    {
        // 1. 保存原始状态
        originalOrthoSize = vcam.m_Lens.OrthographicSize;

        // 2. 停止跟随玩家
        vcam.Follow = null;

        // 3. 平滑移动到目标位置和缩放
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        float startSize = originalOrthoSize;

        while (elapsed < startDuration)
        {
            float t = elapsed / startDuration;
            t = Mathf.SmoothStep(0, 1, t);
            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, orthoSize, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保到达目标状态
        transform.position = targetPosition;
        vcam.m_Lens.OrthographicSize = orthoSize;

        // 4. 持续显示
        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);
        }

        // 5. 平滑恢复
        yield return StartCoroutine(EndCinematicCoroutine(endDuration));

        cinematicCoroutine = null;
    }

    private IEnumerator EndCinematicCoroutine(float endDuration)
    {
        // 获取玩家Transform
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("[VirtualCameraController] 未找到Player标签的对象，无法恢复跟随");
            cinematicCoroutine = null;
            yield break;
        }

        Transform player = playerObj.transform;

        // 停止跟随，手动控制位置进行平滑过渡
        vcam.Follow = null;

        // 平滑过渡：位置和缩放
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        float startSize = vcam.m_Lens.OrthographicSize;
        // 目标位置是玩家位置，但保持当前的 Z 值
        Vector3 targetPos = new Vector3(player.position.x, player.position.y, transform.position.z);

        while (elapsed < endDuration)
        {
            float t = elapsed / endDuration;
            t = Mathf.SmoothStep(0, 1, t);

            // 平滑移动位置
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            // 平滑恢复缩放
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, originalOrthoSize, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保到达目标状态
        transform.position = targetPos;
        vcam.m_Lens.OrthographicSize = originalOrthoSize;

        // 恢复跟随玩家
        vcam.Follow = player;
    }

    #endregion
}
