using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 拉杆开关：支持物体旋转/移动、可选的过场动画和玩家输入锁定
/// 玩家靠近 + 按E键触发
/// </summary>
public class SwitchRotator2DSpecial : MonoBehaviour
{
    [Header("目标物体设置")]
    [Tooltip("要操作的目标物体")]
    public Transform targetObject;

    [Header("旋转设置")]
    [Tooltip("是否启用旋转功能")]
    [SerializeField] private bool enableRotation = true;

    [Tooltip("旋转持续时间 (秒)")]
    [SerializeField] private float rotationDuration = 2.0f;

    [Tooltip("旋转角度 (x, y, z) - 2D游戏通常绕Z轴旋转")]
    [SerializeField] private Vector3 rotationAngles = new Vector3(0, 0, 90);

    [Header("位置移动设置")]
    [Tooltip("是否启用位置移动功能")]
    [SerializeField] private bool enablePositionMove = false;

    [Tooltip("移动目标位置（相对当前位置的偏移）")]
    [SerializeField] private Vector3 positionOffset = Vector3.zero;

    [Tooltip("是否使用世界坐标（false则使用相对坐标）")]
    [SerializeField] private bool useWorldPosition = false;

    [Tooltip("移动目标位置（世界坐标，仅在UseWorldPosition=true时有效）")]
    [SerializeField] private Vector3 targetWorldPosition = Vector3.zero;

    [Tooltip("移动持续时间 (秒)")]
    [SerializeField] private float moveDuration = 2.0f;

    [Header("过场相机设置")]
    [Tooltip("是否启用相机移动功能")]
    [SerializeField] private bool enableCameraMovement = false;

    [Tooltip("相机目标位置（世界坐标 X, Y, Z）")]
    [SerializeField] private Vector3 cameraTargetPosition = Vector3.zero;

    [Tooltip("相机缩放大小（值越大视野越大）")]
    [SerializeField] private float cameraOrthoSize = 8f;

    [Tooltip("相机过场持续时间（秒）")]
    [SerializeField] private float cinematicDuration = 3f;

    [Tooltip("相机移动至目标的时间（秒）")]
    [SerializeField] private float startDuration = 1f;

    [Tooltip("相机从目标返回的时间（秒）")]
    [SerializeField] private float endDuration = 1f;

    [Header("玩家控制设置")]
    [Tooltip("是否在触发时锁定玩家输入")]
    [SerializeField] private bool lockPlayerInput = false;

    [Header("通用设置")]
    public bool oneTimeOnly = false;

    [Header("调试")]
    [Tooltip("显示调试日志")]
    [SerializeField] private bool debugMode = false;


    private Animator _animator;
    private static readonly int LeftOPTrigger = Animator.StringToHash("leftOperate");
    private static readonly int RightOPTrigger = Animator.StringToHash("rightOperate");

    // 状态变量
    private bool isPlayerNearby = false;
    private bool isMoving = false;
    private bool hasTriggered = false;
    private PlayerController playerController;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("[TrampolineAnim] 未找到_animator组件！", this);
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            if (oneTimeOnly && hasTriggered) return;
            StartCoroutine(TriggerSequence());
        }
    }

    /// <summary>
    /// 触发序列：旋转/移动 + 可选的相机移动 + 可选的玩家锁定
    /// </summary>
    private IEnumerator TriggerSequence()
    {
        isMoving = true;
        hasTriggered = true;

        // 根据玩家位置触发拉杆动画
        if (_animator != null)
        {
            float playerX = playerController != null ? playerController.transform.position.x : 0f;
            float switchX = transform.position.x;

            if (playerX < switchX)
            {
                _animator.SetTrigger(LeftOPTrigger);
                if (debugMode) Debug.Log("[SwitchRotator2D] 触发 leftOP 动画（玩家在左侧）");
            }
            else
            {
                _animator.SetTrigger(RightOPTrigger);
                if (debugMode) Debug.Log("[SwitchRotator2D] 触发 rightOP 动画（玩家在右侧）");
            }
        }

        if (debugMode) Debug.Log("[SwitchRotator2D] 触发序列开始");

        // 1. 锁定玩家输入（如果启用）
        if (lockPlayerInput && playerController != null)
        {
            // 先切换到 Idle 状态（IdleState 会自动清空 X 轴速度）
            playerController.StateMachine.ChangeState(playerController.IdleState);

            // 再锁定输入和清空速度
            playerController.IsInputLocked = true;
            playerController.rb.velocity = Vector2.zero;
            playerController.rb.angularVelocity = 0f;
            if (debugMode) Debug.Log("[SwitchRotator2DSpecial] 玩家输入已锁定，状态切换至 Idle");
        }

        // 2. 启动相机移动（如果启用）
        if (enableCameraMovement)
        {
            if (VirtualCameraController.Instance != null)
            {
                VirtualCameraController.Instance.StartCinematic(cameraTargetPosition, cameraOrthoSize, cinematicDuration, startDuration, endDuration);
                if (debugMode) Debug.Log("[SwitchRotator2D] 相机过场已启动");
            }
            else
            {
                Debug.LogError("[SwitchRotator2D] VirtualCameraController.Instance 不存在！请确保场景中有 VirtualCameraController");
                enableCameraMovement = false;
            }
        }

        // 3. 同时执行旋转和位置移动
        Coroutine rotationCoroutine = null;
        Coroutine moveCoroutine = null;

        if (enableRotation)
        {
            rotationCoroutine = StartCoroutine(RotateObject());
        }

        if (enablePositionMove)
        {
            moveCoroutine = StartCoroutine(MoveObject());
        }

        // 等待所有操作完成
        if (rotationCoroutine != null) yield return rotationCoroutine;
        if (moveCoroutine != null) yield return moveCoroutine;

        if (debugMode) Debug.Log("[SwitchRotator2D] 物体操作完成");

        // 4. 等待相机过场完成（如果启用了相机移动）
        if (enableCameraMovement)
        {
            yield return new WaitForSeconds(cinematicDuration);
            if (debugMode) Debug.Log("[SwitchRotator2D] 相机过场完成");
        }

        // 5. 解锁玩家输入（如果之前锁定了）
        if (lockPlayerInput && playerController != null)
        {
            playerController.IsInputLocked = false;
            if (debugMode) Debug.Log("[SwitchRotator2D] 玩家输入已解锁");
        }

        isMoving = false;
        if (debugMode) Debug.Log("[SwitchRotator2D] 触发序列结束");
    }

    /// <summary>
    /// 旋转物体
    /// </summary>
    private IEnumerator RotateObject()
    {
        if (debugMode) Debug.Log("[SwitchRotator2D] 开始旋转");

        Quaternion startRotation = targetObject.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(rotationAngles);
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            t = Mathf.SmoothStep(0, 1, t);
            targetObject.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            playerController.transform.rotation = Quaternion.identity;//保持玩家直立
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetObject.rotation = endRotation;
        if (debugMode) Debug.Log("[SwitchRotator2D] 旋转完成");
    }

    /// <summary>
    /// 移动物体
    /// </summary>
    private IEnumerator MoveObject()
    {
        if (debugMode) Debug.Log("[SwitchRotator2D] 开始移动");
        Vector3 startPos = targetObject.position;
        Vector3 endPos;

        if (useWorldPosition)
        {
            endPos = targetWorldPosition;
        }
        else
        {
            endPos = startPos + positionOffset;
        }

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            t = Mathf.SmoothStep(0, 1, t);
            targetObject.position = Vector3.Lerp(startPos, endPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetObject.position = endPos;
        if (debugMode) Debug.Log("[SwitchRotator2D] 移动完成");



    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerController = other.GetComponent<PlayerController>();

            if (debugMode) Debug.Log("[SwitchRotator2D] 玩家进入触发区域，按E键触发");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (debugMode) Debug.Log("[SwitchRotator2D] 玩家离开触发区域");
        }
    }

    /// <summary>
    /// 编辑器辅助：在Scene视图中绘制辅助信息
    /// </summary>
    private void OnDrawGizmos()
    {
        // 绘制触发区域
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0.8f, 0f, 0.3f); // 橙色半透明
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }

        // 绘制相机目标位置
        if (enableCameraMovement)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cameraTargetPosition, 0.5f);
            Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
            Gizmos.DrawLine(transform.position, cameraTargetPosition);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(cameraTargetPosition, "相机目标");
#endif
        }

        // 绘制物体移动目标位置
        if (enablePositionMove && targetObject != null)
        {
            Vector3 endPos = useWorldPosition ? targetWorldPosition : targetObject.position + positionOffset;

            // 绿色箭头表示移动方向
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(endPos, 0.3f);
            Gizmos.DrawLine(targetObject.position, endPos);

            // 绘制箭头头部
            Vector3 direction = (endPos - targetObject.position).normalized;
            Vector3 arrowPos = endPos - direction * 0.2f;
            Gizmos.DrawLine(arrowPos, endPos);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(endPos, useWorldPosition ? "移动目标(世界)" : "移动目标(相对)");
#endif
        }
    }

    /// <summary>
    /// 重置触发状态
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        isMoving = false;
        if (debugMode) Debug.Log("[SwitchRotator2D] 开关已重置");
    }
}
