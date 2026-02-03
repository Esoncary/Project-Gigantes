using System.Collections;
using UnityEngine;

/// <summary>
/// 拉杆开关：支持物体旋转、可选的过场动画和玩家输入锁定
/// 玩家靠近 + 按E键触发
/// </summary>
public class SwitchRotator2D : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("要旋转的目标物体")]
    public Transform targetObject;

    [Tooltip("旋转持续时间 (秒)")]
    public float duration = 2.0f;

    [Tooltip("旋转角度 (x, y, z) - 2D游戏通常绕Z轴旋转")]
    public Vector3 rotationAngles = new Vector3(0, 0, 90);

    [Header("过场相机设置")]
    [Tooltip("是否启用相机移动功能")]
    [SerializeField] private bool enableCameraMovement = false;

    [Tooltip("相机目标位置（世界坐标 X, Y, Z）")]
    [SerializeField] private Vector3 cameraTargetPosition = Vector3.zero;

    [Tooltip("相机缩放大小（值越大视野越大）")]
    [SerializeField] private float cameraOrthoSize = 8f;

    [Tooltip("相机过场持续时间（秒）")] [SerializeField] private float cinematicDuration = 3f;
    [Tooltip("相机移动至目标的时间（秒）")] [SerializeField] private float startDuration = 1f;
    [Tooltip("相机从目标返回的时间（秒）")][SerializeField] private float endDuration = 1f;

    [Header("玩家控制设置")]
    [Tooltip("是否在触发时锁定玩家输入")]
    [SerializeField] private bool lockPlayerInput = false;

    [Header("通用设置")]
    public bool oneTimeOnly = false;

    [Header("调试")]
    [Tooltip("显示调试日志")]
    [SerializeField] private bool debugMode = false;

    // 状态变量
    private bool isPlayerNearby = false;
    private bool isMoving = false;
    private bool hasTriggered = false;
    private PlayerController playerController;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            if (oneTimeOnly && hasTriggered) return;
            StartCoroutine(TriggerSequence());
        }
    }

    /// <summary>
    /// 触发序列：旋转 + 可选的相机移动 + 可选的玩家锁定
    /// </summary>
    private IEnumerator TriggerSequence()
    {
        isMoving = true;
        hasTriggered = true;

        if (debugMode) Debug.Log("[SwitchRotator2D] 触发序列开始");

        // 1. 锁定玩家输入（如果启用）
        if (lockPlayerInput && playerController != null)
        {
            playerController.IsInputLocked = true;
            playerController.rb.velocity = Vector2.zero;
            if (debugMode) Debug.Log("[SwitchRotator2D] 玩家输入已锁定");
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
                enableCameraMovement = false; // 禁用相机移动，避免后续逻辑出错
            }
        }

        // 3. 执行物体旋转
        Quaternion startRotation = targetObject.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(rotationAngles);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            targetObject.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetObject.rotation = endRotation;
        if (debugMode) Debug.Log("[SwitchRotator2D] 旋转完成");

        // 4. 等待相机过场完成（如果启用了相机移动）
        if (enableCameraMovement)
        {
            float totalWaitTime = cinematicDuration;
            yield return new WaitForSeconds(totalWaitTime);
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
    /// 编辑器辅助：在Scene视图中绘制相机目标位置
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

        // 绘制相机目标位置（仅当启用相机移动时）
        if (enableCameraMovement)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cameraTargetPosition, 0.5f);

            // 绘制从开关到相机目标的连线
            Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
            Gizmos.DrawLine(transform.position, cameraTargetPosition);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(cameraTargetPosition, "相机目标位置");
#endif
        }
    }
}