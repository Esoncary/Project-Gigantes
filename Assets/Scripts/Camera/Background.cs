using UnityEngine;

public class AbsoluteParallax : MonoBehaviour
{
    private Transform camTransform;

    [Header("视差强度")]
    [Range(0, 1)]
    public float parallaxEffect = 0.9f;

    [Header("相对距离限制")]
    [Tooltip("背景偏离相机中心的X轴最大距离")]
    public float limitX = 5f;
    [Tooltip("背景偏离相机中心的Y轴最大距离")]
    public float limitY = 3f;

    private Vector3 bgDesignPos;
    private Vector3 camCapturePos;
    private bool isInitialized = false;

    void Awake()
    {
        bgDesignPos = transform.position;
    }

    void LateUpdate()
    {
        // 1. 获取相机
        if (camTransform == null)
        {
            if (VirtualCameraController.Instance != null)
                camTransform = Camera.main != null ? Camera.main.transform : VirtualCameraController.Instance.transform;
            return;
        }

        // 2. 初始化检查（等相机离开原点）
        if (!isInitialized)
        {
            if (camTransform.position.sqrMagnitude > 0.1f)
            {
                camCapturePos = camTransform.position;
                isInitialized = true;
            }
            return;
        }

        // 3. 计算【视差目标点】
        Vector3 camDelta = camTransform.position - camCapturePos;
        Vector3 targetPos = new Vector3(
            bgDesignPos.x + (camDelta.x * parallaxEffect),
            bgDesignPos.y + (camDelta.y * parallaxEffect),
            bgDesignPos.z
        );

        // 4. 【核心逻辑】限制相对距离
        // 计算目标点相对于相机中心的位置
        float offsetX = targetPos.x - camTransform.position.x;
        float offsetY = targetPos.y - camTransform.position.y;

        // 对偏移量进行限制（Clamp）
        float clampedX = Mathf.Clamp(offsetX, -limitX, limitX);
        float clampedY = Mathf.Clamp(offsetY, -limitY, limitY);

        // 5. 应用位置
        // 最终位置 = 相机位置 + 限制后的偏移量
        transform.position = new Vector3(
            camTransform.position.x + clampedX,
            camTransform.position.y + clampedY,
            targetPos.z
        );
    }
}