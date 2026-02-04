using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenePanel : BasePanel
{
    // UI控件引用
    public Button leftBtn;
    public Button rightBtn;
    public Button beginBtn;
    public Button bakcBtn;
    public Button settingBtn;
    public Button questionBtn;
    public Button level1;
    public Button level2;
    public Button level3;
    public Button level4;

    // 分页配置
    private int pageIndex = 0;
    private const int PageSize = 4;

    // 关卡按钮管理
    private List<Button> levelButtons = new List<Button>();
    private Button selectedLevelBtn;
    // public int GameDataMgr.Instance.selectedLevelIndex = 0;
    private Vector3[] levelButtonOriginalScales;

    // 选中样式配置
    public float selectedScale = 1.1f;
    public Color selectedBgColor = new Color(1f, 0.8f, 0f);
    public Color normalBgColor = Color.white;
    public Color borderColor = new Color(1f, 0.9f, 0.2f);
    public float breathScaleRange = 0.01f;
    public float breathDuration = 1f;

    // 动画协程管理
    private Coroutine currentBreathCoroutine;
    private Dictionary<Transform, Coroutine> buttonHoverCoroutines = new Dictionary<Transform, Coroutine>();

    // 悬停动画配置
    [Header("关卡按钮悬停动画")]
    [Tooltip("鼠标悬停放大倍数")]
    public float hoverScaleMultiplier = 1.15f;
    [Tooltip("动画过渡时长(秒)")]
    public float hoverAnimationDuration = 0.2f;


    public override void Init()
    {
        CheckSuspendedRecord();

        // 初始化关卡按钮列表
        levelButtons.Add(level1);
        levelButtons.Add(level2);
        levelButtons.Add(level3);
        levelButtons.Add(level4);

        //初始化原始缩放 & 轴心设置
        levelButtonOriginalScales = new Vector3[levelButtons.Count];
        InitLevelButtonScaleAndPivot();

        // 绑定所有交互事件
        BindAllButtonEvents();

        // 初始化默认选中关卡与分页
        InitDefaultSelectedLevel();

        // 刷新界面显示
        RefreshLevelButtons();
    }
    private void BindAllButtonEvents()
    {
        // 绑定关卡按钮点击+悬停事件
        for (int i = 0; i < levelButtons.Count; i++)
        {
            int btnIndex = i;
            levelButtons[i].onClick.AddListener(() =>
            {
                OnLevelButtonClick(btnIndex);
                SoundEffectMgr.Instance.PlaySound("UI/button_click");
            });
            AddLevelButtonHoverEffect(levelButtons[i], levelButtonOriginalScales[i], i);
        }

        // 左按钮
        leftBtn.onClick.AddListener(() =>
        {
            ShowPrevSceneInfo();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
        // 右按钮
        rightBtn.onClick.AddListener(() =>
        {
            ShowNextSceneInfo();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
        // 开始按钮
        beginBtn.onClick.AddListener(() =>
        {
            if (GameDataMgr.Instance.selectedLevelIndex < 0 || GameDataMgr.Instance.selectedLevelIndex >= GameDataMgr.Instance.list_LevelData.Count)
            {
                Debug.Log("选中的关卡索引错误");
                return;
            }
            Debug.Log("开始游戏：" + GameDataMgr.Instance.selectedLevelIndex);
            // UI处理
            UIManager.Instance.HidePanel<ScenePanel>();
            // 逻辑处理
            SceneMgr.Instance.LoadGameScene(GameDataMgr.Instance.selectedLevelIndex);
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
        // 返回按钮
        bakcBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.HidePanel<ScenePanel>();
            UIManager.Instance.ShowPanel<BeginPanel>();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
        settingBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.ShowPanel<SettingPanel>();
            UIManager.Instance.GetPanel<SettingPanel>().HideBtn();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
        questionBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.ShowPanel<QuestionPanel>();
            SoundEffectMgr.Instance.PlaySound("UI/button_click");
        });
    }

    private void InitDefaultSelectedLevel()
    {

        int maxUnlockedId = GameDataMgr.Instance.currentSave.maxUnlockedLevelId;
        int totalLevels = GameDataMgr.Instance.list_LevelData.Count;
        Debug.Log("totalLevels:" + totalLevels);
        Debug.Log("maxUnlockedId:" + maxUnlockedId);
        if (maxUnlockedId >= 0 && maxUnlockedId < totalLevels)
        {
            GameDataMgr.Instance.selectedLevelIndex = maxUnlockedId;
            Debug.Log($"已解锁关卡: {GameDataMgr.Instance.selectedLevelIndex}");
            pageIndex = GameDataMgr.Instance.selectedLevelIndex / PageSize;
        }
    }

    private void OnDestroy()
    {
        // 停止所有运行中协程，防止内存泄漏
        if (currentBreathCoroutine != null)
        {
            StopCoroutine(currentBreathCoroutine);
        }
        foreach (var coroutine in buttonHoverCoroutines.Values)
        {
            if (coroutine != null) StopCoroutine(coroutine);
        }
        buttonHoverCoroutines.Clear();

        // 移除所有按钮事件监听，防止内存泄漏
        RemoveAllButtonListeners();
    }

    // 切换到下一页
    public void ShowNextSceneInfo()
    {
        int totalPages = Mathf.CeilToInt(GameDataMgr.Instance.list_LevelData.Count / (float)PageSize);
        pageIndex++;
        pageIndex = pageIndex >= totalPages ? 0 : pageIndex;
        GameDataMgr.Instance.selectedLevelIndex = -1;
        RefreshLevelButtons();
    }

    // 切换到上一页
    public void ShowPrevSceneInfo()
    {
        int totalPages = Mathf.CeilToInt(GameDataMgr.Instance.list_LevelData.Count / (float)PageSize);
        pageIndex--;
        pageIndex = pageIndex < 0 ? totalPages - 1 : pageIndex;
        GameDataMgr.Instance.selectedLevelIndex = -1;
        RefreshLevelButtons();
    }

    // 刷新当前页的关卡按钮显示
    private void RefreshLevelButtons()
    {
        if (selectedLevelBtn != null)
        {

            ResetLevelButtonStyle(selectedLevelBtn);
            selectedLevelBtn = null; // 清空选中按钮引用
        }

        int startIndex = pageIndex * PageSize;
        int maxUnLockedId = GameDataMgr.Instance.currentSave.maxUnlockedLevelId;

        for (int i = 0; i < levelButtons.Count; i++)
        {
            int levelIndex = startIndex + i;
            Button btn = levelButtons[i];
            Image btnImage = btn.GetComponent<Image>();

            ResetLevelButtonStyle(btn);

            if (levelIndex < GameDataMgr.Instance.list_LevelData.Count)
            {
                // 关卡存在，显示按钮
                btn.gameObject.SetActive(true);
                LevelData levelData = GameDataMgr.Instance.list_LevelData[levelIndex];

                // 设置关卡图片
                if (btnImage != null)
                {
                    btnImage.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(levelData.imgRes);

                }
                btn.transition = Selectable.Transition.SpriteSwap;
                Sprite normalSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(levelData.imgRes);
                Sprite highlightedSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(levelData.imgRes.Replace("关卡", "发光关卡"));

                SpriteState ss = new SpriteState();
                ss.highlightedSprite = highlightedSprite;
                btn.spriteState = ss;


                if (levelIndex > maxUnLockedId)
                {
                    // 未解锁：设置灰色遮罩 + 不可交互
                    // btn.interactable = false;
                    SetButtonGray(btnImage, true);
                }
                else
                {
                    // 已解锁：恢复正常颜色 + 可交互
                    btn.interactable = true;
                    SetButtonGray(btnImage, false);

                    // 同步选中状态
                    if (levelIndex == GameDataMgr.Instance.selectedLevelIndex)
                    {
                        SetLevelButtonSelected(btn);
                    }
                    // 未选中的已解锁按钮，保持默认样式（上面已重置）
                }
            }
            else
            {
                // 关卡不存在，隐藏按钮
                btn.gameObject.SetActive(false);
            }
        }

        // 自动选中当前页第一个可交互关卡
        if (selectedLevelBtn == null)
        {
            // 遍历当前页按钮，匹配我们预设的 GameDataMgr.Instance.selectedLevelIndex
            for (int i = 0; i < levelButtons.Count; i++)
            {
                int levelIndex = startIndex + i;
                if (levelIndex == GameDataMgr.Instance.selectedLevelIndex)
                {
                    OnLevelButtonClick(i);
                    break;
                }
            }
        }
    }
    // 关卡按钮点击事件
    private void OnLevelButtonClick(int btnIndex)
    {
        int startIndex = pageIndex * PageSize;
        int levelIndex = startIndex + btnIndex;

        int maxUnLockedId = GameDataMgr.Instance.currentSave.maxUnlockedLevelId;
        if (levelIndex < 0 || levelIndex >= GameDataMgr.Instance.list_LevelData.Count || levelIndex > maxUnLockedId)
        {
            Debug.Log("关卡未解锁或索引错误，无法选中");
            return;
        }

        // 更新选中关卡索引
        GameDataMgr.Instance.selectedLevelIndex = levelIndex;
        // 更新按钮选中样式
        SetLevelButtonSelected(levelButtons[btnIndex]);
        // 更新开始按钮可交互状态
        beginBtn.interactable = true;
        // 刷新关卡详情
        GetCurrentSceneData();
        Debug.Log("索引为：" + GameDataMgr.Instance.selectedLevelIndex);
    }

    // 设置关卡按钮选中样式
    private void SetLevelButtonSelected(Button btn)
    {
        // 重置上一个选中按钮的样式
        if (selectedLevelBtn != null && selectedLevelBtn != btn)
        {
            ResetLevelButtonStyle(selectedLevelBtn);
        }
        selectedLevelBtn = btn;

        // 停止上一个按钮的呼吸协程，避免残留缩放
        if (currentBreathCoroutine != null)
        {
            StopCoroutine(currentBreathCoroutine);
            currentBreathCoroutine = null;
        }

        // 3. 轻微缩放（1.1倍），视觉突出
        btn.transform.localScale = Vector3.one * selectedScale;

        // 4. 启动呼吸效果（仅已解锁选中的按钮）
        currentBreathCoroutine = StartCoroutine(ScaleBreathe(btn));
    }

    private IEnumerator ScaleBreathe(Button btn)
    {
        // 记录初始缩放，确保基于选中状态的基础缩放波动
        float baseScale = selectedScale;
        // 定义缩放的最小值和最大值
        float minScale = baseScale;
        float maxScale = baseScale + breathScaleRange;

        // 循环计时变量（避免用Time.time导致起始值混乱）
        float elapsedTime = 0f;

        // 只要按钮还是选中状态，就持续呼吸
        while (btn == selectedLevelBtn)
        {
            // 累加时间（基于deltaTime，不受帧率影响）
            elapsedTime += Time.deltaTime;
            // 计算0~1之间的循环值（PingPong让数值在0和1之间来回）
            float t = Mathf.PingPong(elapsedTime / breathDuration, 1f);
            // 平滑插值：minScale → maxScale → minScale 循环
            float currentScale = Mathf.Lerp(minScale, maxScale, t);

            // 应用缩放到整个按钮（Transform控制整体大小）
            btn.transform.localScale = Vector3.one * currentScale;

            // 等待下一帧
            yield return null;
        }

        // 退出循环后，恢复到选中的基础缩放（避免按钮停在放大/缩小状态）
        btn.transform.localScale = Vector3.one * baseScale;
    }

    // 重置关卡按钮默认样式（已解锁未选中/未解锁）
    private void ResetLevelButtonStyle(Button btn)
    {
        // 停止该按钮的呼吸协程
        if (currentBreathCoroutine != null && btn == selectedLevelBtn)
        {
            StopCoroutine(currentBreathCoroutine);
            currentBreathCoroutine = null;
        }

        Image img = btn.GetComponent<Image>();
        if (img != null)
        {
            img.color = normalBgColor; // 默认白色背景
        }
        // 恢复默认缩放
        btn.transform.localScale = Vector3.one;
    }

    // ========== 新增：设置按钮灰色/恢复原色 ==========
    private void SetButtonGray(Image img, bool isGray)
    {
        if (img == null) return;

        if (isGray)
        {
            // 灰色遮罩：降低亮度 + 饱和度
            img.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 纯灰色
            // 进阶方案：使用ColorFilter（效果更自然）
            // img.material = new Material(Shader.Find("UI/Default"));
            // img.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f));
        }
        else
        {
            // 恢复原色
            img.color = normalBgColor;
            // 进阶方案恢复
            // img.material = null;
        }
    }

    // 获取当前选中关卡数据
    public void GetCurrentSceneData()
    {
        if (GameDataMgr.Instance.selectedLevelIndex < 0 || GameDataMgr.Instance.selectedLevelIndex >= GameDataMgr.Instance.list_LevelData.Count)
            return;

        LevelData sceneInfo = SceneMgr.Instance.GetSceneData(GameDataMgr.Instance.selectedLevelIndex);
        // 这里可以扩展显示关卡详情（如收集物数量等）
    }

    // 检查是否有中断记录
    public void CheckSuspendedRecord()
    {
        if (GameDataMgr.Instance.currentSave.suspendData.hasSuspendedRecord)
        {
            UIManager.Instance.ShowPanel<SuspendedPanel>();
        }
    }
    public void SelectNextLevel()
    {
        int totalLevels = GameDataMgr.Instance.list_LevelData.Count;
        // 核心逻辑：当前索引 + 1 = 目标下一关
        int nextLevelIndex = GameDataMgr.Instance.selectedLevelIndex + 1;
        Debug.Log("000:" + GameDataMgr.Instance.selectedLevelIndex);
        // 边界处理（二选一，根据你的游戏需求）
        // 方案1：循环模式（最后一关的下一关回到第一关）
        if (nextLevelIndex >= totalLevels)
        {
            nextLevelIndex = 0;
        }
        if (nextLevelIndex > GameDataMgr.Instance.currentSave.maxUnlockedLevelId)
        {
            GameDataMgr.Instance.currentSave.maxUnlockedLevelId = nextLevelIndex;
            GameDataMgr.Instance.SavePlayerSaveData();
        }
        // 计算目标关卡所在分页
        pageIndex = nextLevelIndex / PageSize;
        // 设置目标选中索引，刷新界面（自动处理样式、分页、呼吸效果）
        GameDataMgr.Instance.selectedLevelIndex = nextLevelIndex;
        RefreshLevelButtons();

    }
    private void InitLevelButtonScaleAndPivot()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            Button btn = levelButtons[i];
            if (btn == null) continue;

            // 记录原始缩放
            levelButtonOriginalScales[i] = btn.transform.localScale;

            // 强制设置轴心为中心点，保证中心缩放
            RectTransform rectTrans = btn.GetComponent<RectTransform>();
            if (rectTrans != null)
            {
                rectTrans.pivot = new Vector2(0.5f, 0.5f);
            }
        }
    }

    // ====================== 新增：为关卡按钮绑定悬停/移出动画 ======================
    private void AddLevelButtonHoverEffect(Button targetBtn, Vector3 originScale, int btnListIndex)
    {
        if (targetBtn == null) return;

        EventTrigger trigger = targetBtn.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = targetBtn.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        Transform targetTrans = targetBtn.transform;

        // 鼠标移入事件
        EventTrigger.Entry enterEvent = new EventTrigger.Entry();
        enterEvent.eventID = EventTriggerType.PointerEnter;
        enterEvent.callback.AddListener((data) =>
        {
            // 过滤条件：选中状态 / 未解锁 不执行动画
            if (targetBtn == selectedLevelBtn || !targetBtn.interactable) return;

            StopTargetHoverCoroutine(targetTrans);
            buttonHoverCoroutines[targetTrans] = StartCoroutine(ScaleLerp(targetTrans, originScale * hoverScaleMultiplier));
        });
        trigger.triggers.Add(enterEvent);

        // 鼠标移出事件
        EventTrigger.Entry exitEvent = new EventTrigger.Entry();
        exitEvent.eventID = EventTriggerType.PointerExit;
        exitEvent.callback.AddListener((data) =>
        {
            // 过滤条件：选中状态 不执行动画
            if (targetBtn == selectedLevelBtn) return;

            StopTargetHoverCoroutine(targetTrans);
            buttonHoverCoroutines[targetTrans] = StartCoroutine(ScaleLerp(targetTrans, originScale));
        });
        trigger.triggers.Add(exitEvent);
    }

    // ====================== 新增：停止指定按钮的悬停协程 ======================
    private void StopTargetHoverCoroutine(Transform targetTrans)
    {
        if (buttonHoverCoroutines.ContainsKey(targetTrans) && buttonHoverCoroutines[targetTrans] != null)
        {
            StopCoroutine(buttonHoverCoroutines[targetTrans]);
        }
    }

    // ====================== 新增：平滑缩放协程 ======================
    private IEnumerator ScaleLerp(Transform targetTrans, Vector3 targetScale)
    {
        Vector3 startScale = targetTrans.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < hoverAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / hoverAnimationDuration);
            targetTrans.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return null;
        }
        // 保证最终缩放精度
        targetTrans.localScale = targetScale;
    }
    private void RemoveAllButtonListeners()
    {
        leftBtn.onClick.RemoveAllListeners();
        rightBtn.onClick.RemoveAllListeners();
        beginBtn.onClick.RemoveAllListeners();
        bakcBtn.onClick.RemoveAllListeners();
        settingBtn.onClick.RemoveAllListeners();
        questionBtn.onClick.RemoveAllListeners();

        foreach (var btn in levelButtons)
        {
            btn.onClick.RemoveAllListeners();
        }
    }
}