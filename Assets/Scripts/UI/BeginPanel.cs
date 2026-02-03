using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button startBtn;
    public Button settingBtn;
    public Button quitBtn;

    [Header("按钮缩放动画配置")]
    [Tooltip("鼠标悬停的放大倍数")]
    public float scaleMultiplier = 1.2f;
    [Tooltip("动画过渡时间，值越小动画越快")]
    public float animationDuration = 0.2f;

    // 存储所有按钮的原始缩放值
    private Vector3 startBtnOriginScale;
    private Vector3 settingBtnOriginScale;
    private Vector3 quitBtnOriginScale;

    // 核心优化：用字典独立管理每个按钮的协程，避免多按钮动画冲突
    private Dictionary<Transform, Coroutine> buttonScaleCoroutines = new Dictionary<Transform, Coroutine>();


    public override void Init()
    {
        // 初始化记录按钮原始缩放
        InitButtonScale();

        // 为三个按钮统一添加悬停放大/移出还原动画
        AddScaleAnimation(startBtn, startBtnOriginScale);
        AddScaleAnimation(settingBtn, settingBtnOriginScale);
        AddScaleAnimation(quitBtn, quitBtnOriginScale);
        // 开始按钮
        startBtn.onClick.AddListener(() =>
        {
            Debug.Log("kaishiyouxi ");
            // UI处理
            UIManager.Instance.HidePanel<BeginPanel>();
            UIManager.Instance.ShowPanel<ScenePanel>();

            // 默认选择存档1

            GameDataMgr.Instance.LoadChoosePlayerSaveData(0);
            // GameDataMgr.Instance.currentSave.ToString();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");

        });

        // 设置按钮
        settingBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.ShowPanel<SettingPanel>();
            UIManager.Instance.GetPanel<SettingPanel>().HideBtn();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });

        // 退出按钮
        quitBtn.onClick.AddListener(() =>
        {
            Debug.Log("Quit");
            GameDataMgr.Instance.SavePlayerSaveData();
            Application.Quit();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
    }
    /// <summary>
    /// 记录所有按钮的初始缩放值
    /// </summary>
    private void InitButtonScale()
    {
        startBtnOriginScale = startBtn.transform.localScale;
        settingBtnOriginScale = settingBtn.transform.localScale;
        quitBtnOriginScale = quitBtn.transform.localScale;
    }

    /// <summary>
    /// 为按钮添加鼠标移入放大、移出还原的动画
    /// </summary>
    private void AddScaleAnimation(Button targetBtn, Vector3 originScale)
    {
        // 为空校验，防止场景中未绑定按钮报错
        if (targetBtn == null) return;

        Transform targetTrans = targetBtn.transform;
        // 获取/添加EventTrigger组件（监听鼠标UI事件）
        EventTrigger trigger = targetBtn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = targetBtn.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        // 1. 鼠标移入事件：放大按钮
        EventTrigger.Entry enterEvent = new EventTrigger.Entry();
        enterEvent.eventID = EventTriggerType.PointerEnter;
        enterEvent.callback.AddListener((data) =>
        {
            StopTargetCoroutine(targetTrans);
            // 启动协程：平滑放大
            buttonScaleCoroutines[targetTrans] = StartCoroutine(ScaleLerp(targetTrans, originScale * scaleMultiplier));
        });
        trigger.triggers.Add(enterEvent);

        // 2. 鼠标移出事件：恢复原始大小（核心需求）
        EventTrigger.Entry exitEvent = new EventTrigger.Entry();
        exitEvent.eventID = EventTriggerType.PointerExit;
        exitEvent.callback.AddListener((data) =>
        {
            StopTargetCoroutine(targetTrans);
            // 启动协程：平滑还原初始缩放
            buttonScaleCoroutines[targetTrans] = StartCoroutine(ScaleLerp(targetTrans, originScale));
        });
        trigger.triggers.Add(exitEvent);
    }

    /// <summary>
    /// 停止指定按钮的正在运行的缩放协程
    /// </summary>
    private void StopTargetCoroutine(Transform targetTrans)
    {
        if (buttonScaleCoroutines.ContainsKey(targetTrans) && buttonScaleCoroutines[targetTrans] != null)
        {
            StopCoroutine(buttonScaleCoroutines[targetTrans]);
        }
    }

    /// <summary>
    /// 协程：线性插值实现平滑缩放
    /// </summary>
    private IEnumerator ScaleLerp(Transform targetTrans, Vector3 targetScale)
    {
        // 记录动画开始时的缩放值
        Vector3 startScale = targetTrans.localScale;
        float elapsedTime = 0f;

        // 逐帧插值过渡
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / animationDuration);
            targetTrans.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }

        // 动画结束后强制赋值，保证缩放精度
        targetTrans.localScale = targetScale;
    }

    /// <summary>
    /// 面板销毁时，清理所有协程，防止内存泄漏
    /// </summary>
    private void OnDestroy()
    {
        buttonScaleCoroutines.Clear();
    }
}
