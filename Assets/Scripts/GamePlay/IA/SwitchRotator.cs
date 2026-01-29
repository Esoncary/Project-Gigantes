using System.Collections;
using UnityEngine;

public class SwitchRotator2D : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("要旋转的目标物体")]
    public Transform targetObject;

    [Tooltip("旋转持续时间 (x秒)")]
    public float duration = 2.0f;

    // 注意：2D 游戏通常是绕 Z 轴旋转 (0, 0, 90)
    [Tooltip("旋转角度 (x, y, z)")]
    public Vector3 rotationAngles = new Vector3(0, 0, 90);

    [Header("状态")]
    public bool oneTimeOnly = false;

    private bool isPlayerNearby = false;
    private bool isMoving = false;
    private bool hasTriggered = false;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E) && !isMoving)
        {
            if (oneTimeOnly && hasTriggered) return;
            StartCoroutine(RotateObject());
        }
    }

    private IEnumerator RotateObject()
    {
        isMoving = true;
        hasTriggered = true;

        Quaternion startRotation = targetObject.rotation;
        // 2D 旋转通常基于 Z 轴
        Quaternion endRotation = startRotation * Quaternion.Euler(rotationAngles);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            // 平滑插值
            // t = Mathf.SmoothStep(0f, 1f, t); 

            targetObject.rotation = Quaternion.Lerp(startRotation, endRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetObject.rotation = endRotation;
        isMoving = false;
        Debug.Log("2D 旋转完成");
    }

    // ==========================================
    // 关键修改：这里必须加 "2D" 后缀！！！
    // ==========================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("进入触发区域 (2D)");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Debug.Log("离开触发区域 (2D)");
        }
    }
}