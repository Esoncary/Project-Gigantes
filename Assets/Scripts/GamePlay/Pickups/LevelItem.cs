using UnityEngine;

public class LevelItem : MonoBehaviour
{
    [Header("存档设置")]
    public string itemID;

     //... GenerateID 保持不变...

    protected virtual void Start()
    {
        CheckStatus();
    }

    protected void CheckStatus()
    {
        if (string.IsNullOrEmpty(itemID)) return;

        var savedItems = GameDataMgr.Instance.currentSave.suspendData.interactedItems;
        var sessionItems = GameDataMgr.Instance.currentLevelCollectedIds;

        // 如果持久存档或本次内存记录中有这个ID
        if (savedItems.Contains(itemID) || sessionItems.Contains(itemID))
        {
            HandleAlreadyInteracted();
        }
    }

    protected virtual void HandleAlreadyInteracted()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnInteract()
    {
        if (!string.IsNullOrEmpty(itemID))
        {
            // 1. 记录到内存临时表
            // GameDataMgr.Instance.RecordItem(itemID);

            // 2. 如果你希望“实时中断存档”（即捡起瞬间就写进硬盘，防止崩溃丢进度）
            // 可以取消下面两行的注释：
            // GameDataMgr.Instance.currentSave.suspendData.interactedItems.Add(itemID);
            // GameDataMgr.Instance.SavePlayerSaveData();
        }
    }
}