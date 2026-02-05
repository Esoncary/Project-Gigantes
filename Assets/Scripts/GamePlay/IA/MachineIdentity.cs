using UnityEngine;

// 挂载到可存档的机器上，用于唯一标识
public class MachineIdentity : MonoBehaviour
{
    [Header("唯一ID（场景内不可重复）")]
    public string machineUniqueId;

    private void Awake()
    {
        // 防错：空ID自动生成
        if (string.IsNullOrEmpty(machineUniqueId))
        {
            machineUniqueId = System.Guid.NewGuid().ToString();
            Debug.LogWarning($"物体 {gameObject.name} 未设置机器ID，已自动生成：{machineUniqueId}");
        }
    }
}