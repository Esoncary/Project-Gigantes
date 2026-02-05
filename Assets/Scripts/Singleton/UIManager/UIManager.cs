using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class UIManager
{
    //面板存储容器
    Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();
    //面板父对象
    Transform canvasTrans;

    // 输入锁定计数器，支持多层面板叠加
    private int inputLockCount = 0;


    //单例
    private static UIManager instance = new UIManager();
    public static UIManager Instance => instance;
    public UIManager()
    {
        GameObject canvas = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Canvas"));
        canvasTrans = canvas.transform;
        //保证只有一个Canvas
        GameObject.DontDestroyOnLoad(canvas);
    }

    // 获取PlayerController引用（每次重新查找，避免缓存null值）
    private PlayerController GetPlayerController()
    {
        return GameObject.FindObjectOfType<PlayerController>();
    }

    // 更新输入锁定状态
    private void UpdateInputLockState()
    {
        var pc = GetPlayerController();
        if (pc != null)
        {
            pc.IsInputLocked = inputLockCount > 0;
        }
        else
        {
            Debug.LogWarning($"[UIManager] PlayerController not found! InputLockCount: {inputLockCount}");
        }
    }
    // 实例化并显示面板
    public T ShowPanel<T>() where T : BasePanel
    {
        string name = typeof(T).Name;
        if (panelDic.ContainsKey(name))
        {
            return panelDic[name] as T;
        }
        GameObject panelObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/" + name), canvasTrans);
        T panel = panelObj.GetComponent<T>();
        panelDic.Add(name, panel);

        // 输入锁定处理
        if (panel.ShouldLockPlayerInput)
        {
            inputLockCount++;
            UpdateInputLockState();
        }

        panel.ShowMe();
        return panel;
    }
    // 隐藏并销毁面板
    public void HidePanel<T>(bool isFade = true) where T : BasePanel
    {
        string name = typeof(T).Name;
        if (panelDic.ContainsKey(name))
        {
            var panel = panelDic[name];

            // 输入解锁处理
            if (panel.ShouldLockPlayerInput)
            {
                inputLockCount = Mathf.Max(0, inputLockCount - 1);
                UpdateInputLockState();
            }

            //是否需要在谈出结束后删除
            if (isFade)
            {
                panelDic[name].HideMe(() =>
                {
                    GameObject.Destroy(panelDic[name].gameObject);
                    panelDic.Remove(name);
                });
            }
            else
            {
                GameObject.Destroy(panelDic[name].gameObject);
                panelDic.Remove(name);
            }
        }
    }

    // 得到面板
    public T GetPanel<T>() where T : BasePanel
    {
        string name = typeof(T).Name;
        if (panelDic.ContainsKey(name))
            return panelDic[name] as T;
        return null;
    }
    public void HideAllPanel()
    {
        foreach (var panel in panelDic.Values)
        {
            if (panel != null)
            {
                GameObject.Destroy(panel.gameObject);
            }
        }
        panelDic.Clear();
    }



}
