using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraController : MonoBehaviour
{
    #region 单例化
    public static VirtualCameraController Instance { get; private set; }

    private CinemachineConfiner2D confiner;
    private CinemachineFramingTransposer framingTransposer;
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
    
    #region 相机滞后效果

    [Header("释放滞后效果")]
    [SerializeField] private float releaseOffsetIntensity = 2f;  // 滞后强度（偏移量）
    [SerializeField] private float recoverSpeed = 5f;            // 恢复速度

    private Vector3 currentOffset;                              // 当前偏移量

    #endregion


    CinemachineVirtualCamera vcam;

    // 过场动画相关字段
    private float originalOrthoSize;
    private Coroutine cinematicCoroutine;
    private GameObject cinematicFollowTarget;  // 临时跟随目标对象

    // Start is called before the first frame update
    void Start()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
        framingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (confiner != null && confiner.m_BoundingShape2D == null)
        {
            ResetCameraBound();//绑定相机边界
        }

        // 相机偏移平滑恢复
        // 将摄像机从释放时的偏移位置平滑过渡回正常跟随位置，避免突兀的跳回
        if (framingTransposer != null && framingTransposer.m_TrackedObjectOffset != Vector3.zero)
        {
            framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(
                framingTransposer.m_TrackedObjectOffset,
                Vector3.zero,
                Time.deltaTime * recoverSpeed
            );

            // 当偏移量足够小时直接归零，避免无限逼近
            if (framingTransposer.m_TrackedObjectOffset.magnitude < 0.01f)
            {
                framingTransposer.m_TrackedObjectOffset = Vector3.zero;
            }
        }
    }

    private void OnDestroy()
    {
        // 确保临时对象被销毁
        if (cinematicFollowTarget != null)
        {
            Destroy(cinematicFollowTarget);
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

    #region 相机滞后效果

    /// <summary>
    /// 应用释放时的相机滞后效果
    /// </summary>
    /// <param name="releaseDirection">释放方向（归一化的向量）</param>
    /// 释放时摄像机相较于玩家运动方向滞后，增强运动感
    public void ApplyReleaseLag(Vector2 releaseDirection)
    {
        if (framingTransposer == null) return;

        // 核心逻辑：设置反向偏移
        // 例如：向右喷射(1, 0)，偏移量变为(-2, 0)
        currentOffset = -releaseDirection.normalized * releaseOffsetIntensity;
        framingTransposer.m_TrackedObjectOffset = currentOffset;
    }

    #endregion

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
        // 保存原始状态
        originalOrthoSize = vcam.m_Lens.OrthographicSize;

        // 获取玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("[VirtualCameraController] 未找到Player，无法执行过场动画");
            cinematicCoroutine = null;
            yield break;
        }
        Transform player = playerObj.transform;

        // cinematic 的 follow 从 none 变为具体对象时会闪烁
        // 创建临时跟随目标对象（从玩家位置开始）
        cinematicFollowTarget = new GameObject("CinematicFollowTarget");
        cinematicFollowTarget.transform.position = new Vector3(player.position.x, player.position.y, player.position.z);

        // 设置Follow为临时对象
        vcam.Follow = cinematicFollowTarget.transform;

        // 阶段1：移动临时目标到目标位置
        float elapsed = 0f;
        Vector3 startPos = cinematicFollowTarget.transform.position;
        Vector3 targetPos = new Vector3(targetPosition.x, targetPosition.y, player.position.z);

        while (elapsed < startDuration)
        {
            float t = elapsed / startDuration;
            t = Mathf.SmoothStep(0, 1, t);
            cinematicFollowTarget.transform.position = Vector3.Lerp(startPos, targetPos, t);
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(originalOrthoSize, orthoSize, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保到达目标状态
        cinematicFollowTarget.transform.position = targetPos;
        vcam.m_Lens.OrthographicSize = orthoSize;

        // 阶段2：持续显示
        if (duration > 0)
        {
            yield return new WaitForSeconds(duration);
        }

        // 阶段3：平滑恢复（移动临时目标回玩家位置）
        elapsed = 0f;
        startPos = cinematicFollowTarget.transform.position;
        targetPos = new Vector3(player.position.x, player.position.y, player.position.z);

        while (elapsed < endDuration)
        {
            float t = elapsed / endDuration;
            t = Mathf.SmoothStep(0, 1, t);
            cinematicFollowTarget.transform.position = Vector3.Lerp(startPos, targetPos, t);
            vcam.m_Lens.OrthographicSize = Mathf.Lerp(orthoSize, originalOrthoSize, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 确保到达玩家位置
        cinematicFollowTarget.transform.position = targetPos;
        vcam.m_Lens.OrthographicSize = originalOrthoSize;

        // 恢复跟随玩家
        vcam.Follow = player;

        // 销毁临时对象
        if (cinematicFollowTarget != null)
        {
            Destroy(cinematicFollowTarget);
            cinematicFollowTarget = null;
        }

        cinematicCoroutine = null;
    }

    #endregion
}
