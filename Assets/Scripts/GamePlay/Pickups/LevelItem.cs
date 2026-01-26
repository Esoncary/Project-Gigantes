using UnityEngine;
using System;

public class LevelItem : MonoBehaviour
{
    public string itemID;

    [ContextMenu("生成唯一ID")]
    private void GenerateID()
    {
        itemID = System.Guid.NewGuid().ToString();
    }

    private void Start()
    {
        // GenerateID();
        CheckStatus();
    }

    private void CheckStatus()
    {
        var data = GameDataMgr.Instance.currentSave;
        if (data.HasSuspendedRecord && data.SuspendInteractedItems.Contains(itemID))
        {
            HandleAlreadyInteracted();
        }
    }

    // 子类可以重写这个方法（比如宝箱是变成打开状态，怪物是直接销毁）
    protected virtual void HandleAlreadyInteracted()
    {
        gameObject.SetActive(false);
    }

    public void OnInteract()
    {
        // 通知 SceneMgr 记录我的 ID
        SceneMgr.Instance.RecordItem(itemID);

        // 执行原本的消失逻辑
        gameObject.SetActive(false);
    }
}