using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    [Header("绑定引用")]
    public PlayerController player; // 拖入你的玩家对象
    public Image targetFillImage;   // 拖入 TargetFill 图片
    public Image currentFillImage;  // 拖入 CurrentFill 图片

    void Update()
    {
        if (player == null) return;

        // 计算比例：(当前值 / 最大容量)
        // 假设你在 Controller 里定义了 maxStorage
        float targetRatio = player.targetStorage / player.maxStorage;
        float currentRatio = player.currentStorage / player.maxStorage;

        // 更新 UI 填充值 (范围 0 到 1)
        targetFillImage.fillAmount = Mathf.Clamp01(targetRatio);
        currentFillImage.fillAmount = Mathf.Clamp01(currentRatio);
    }
}