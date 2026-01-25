using UnityEngine;
using UnityEngine.UI;

public class StorageUI : MonoBehaviour
{
    [Header("绑定引用")]
    public PlayerController player; // 拖入你的玩家对象
    public Image targetFillImage;   // 拖入 TargetFill 图片
    public Image currentFillImage;  // 拖入 CurrentFill 图片

    void Start()
    {
        player = SceneMgr.Instance.player;
        Debug.Log("123123" + player);
    }
    void Update()
    {
        if (player == null) return;

        // 计算比例：(当前值 / 最大容量)
        // 假设你在 Controller 里定义了 maxStorage
        float targetRatio = player.targetStorage / player.maxStorage;
        float currentRatio = player.currentStorage / player.maxStorage;

        // 更新 UI 填充值 (范围 0 到 1)
        targetFillImage.fillAmount = Mathf.MoveTowards(targetFillImage.fillAmount, Mathf.Clamp01(targetRatio), 1f * Time.deltaTime);//虽然targetStorage在逻辑上是每帧瞬间变动的，但视觉上加个平滑过渡会好看一些，就像DNF里的血条和蓝条
        currentFillImage.fillAmount = Mathf.Clamp01(currentRatio);
    }
}