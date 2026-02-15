using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SuspendedPanel : BasePanel
{
    // 中断记录面板需要锁定玩家输入
    protected override bool LockPlayerInputOnShow => true;

    public Button confirmBtn;
    public Button cancelBtn;
    public override void Init()
    {
        // 继续中断按钮
        confirmBtn.onClick.AddListener(async () =>
        {
            // Ui处理
            UIManager.Instance.HidePanel<SuspendedPanel>();
            

            //260215:增加场景过渡
            await SceneMgr.Instance.SceneTransitionAsync(async () =>
            {
                UIManager.Instance.HidePanel<ScenePanel>();

                int levelId = GameDataMgr.Instance.currentSave.suspendData.suspendLevelId;
                await SceneMgr.Instance.LoadGameScene(levelId);
                
            });

            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
        // 取消中断按钮
        cancelBtn.onClick.AddListener(() =>
        {
            // Ui处理
            UIManager.Instance.HidePanel<SuspendedPanel>();
            // 逻辑处理
            GameDataMgr.Instance.ClearSuspendData();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });

    }

}
