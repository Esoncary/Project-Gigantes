
using UnityEngine;

public class LevelItem : MonoBehaviour
{
    [Header("存档设置")]
    public string itemID;

    // 确保itemID不为空（可选：自动生成唯一ID）
    protected virtual void Awake()
    {

        if (string.IsNullOrEmpty(itemID))
        {
            itemID = System.Guid.NewGuid().ToString();
            Debug.LogWarning($"[{gameObject.name}] 未设置itemID，已自动生成：{itemID}");
        }
    }

    protected virtual void Start()
    {
        CheckStatus();
    }

    protected void CheckStatus()
    {
        if (string.IsNullOrEmpty(itemID)) return;

        var savedItems = GameDataMgr.Instance.currentSave.suspendData.interactedItems;
        var sessionItems = GameDataMgr.Instance.currentLevelCollectedIds;

        // 修复：增加空值判断（防止GameDataMgr未初始化）
        if (savedItems == null || sessionItems == null)
        {
            Debug.LogError("存档数据容器未初始化！");
            return;
        }

        if (savedItems.Contains(itemID) || sessionItems.Contains(itemID))
        {
            Debug.Log($"[{gameObject.name}] 检测到已交互，执行HandleAlreadyInteracted");
            HandleAlreadyInteracted();
        }
    }

    protected virtual void HandleAlreadyInteracted()
    {
        Debug.Log($"[{gameObject.name}] 隐藏已交互物品");
        gameObject.SetActive(false);
    }

    public virtual void OnInteract()
    {
        if (!string.IsNullOrEmpty(itemID))
        {
            // 修复：恢复存档记录逻辑，同时增加空值判断
            GameDataMgr.Instance.RecordItem(itemID);

            // 实时写入持久存档（防止崩溃丢失进度，建议保留）
            // if (!GameDataMgr.Instance.currentSave.suspendData.interactedItems.Contains(itemID))
            // {
            //     GameDataMgr.Instance.currentSave.suspendData.interactedItems.Add(itemID);
            //     GameDataMgr.Instance.SavePlayerSaveData();
            // }
        }
    }
}