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

    [Header("各种UI组件")]
    public Image blackImage;//用来淡入和淡出的黑色图片
    public float blackImageAlpha;//黑幕的alpha值

    //单例
    private static UIManager instance = new UIManager();
    public static UIManager Instance => instance;
    public UIManager()
    {
        GameObject canvas = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/Canvas.prefab"));
        canvasTrans = canvas.transform;
        //保证只有一个Canvas
        GameObject.DontDestroyOnLoad(canvas);
    }
    // 实例化并显示面板
    public T ShowPanel<T>() where T : BasePanel
    {
        string name = typeof(T).Name;
        if (panelDic.ContainsKey(name))
        {
            return panelDic[name] as T;
        }
        GameObject panelObj = GameObject.Instantiate(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/" + name + ".prefab"), canvasTrans);
        T panel = panelObj.GetComponent<T>();
        panelDic.Add(name, panel);
        panel.ShowMe();
        return panel;
    }
    // 隐藏并销毁面板
    public void HidePanel<T>(bool isFade = true) where T : BasePanel
    {
        string name = typeof(T).Name;
        if (panelDic.ContainsKey(name))
        {
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

    //黑屏淡入和淡出函数与协程
    public void StartBlackImageFadeIn()
    {
        // StartCoroutine(BlackImageFadeIn());
        BlackImageFadeIn();
    }
    public void StartBlackImageFadeOut()
    {
        // StartCoroutine(BlackImageFadeOut());
        BlackImageFadeOut();
    }

    public async void BlackImageFadeOut()
    {
        blackImageAlpha = 1;
        while (blackImageAlpha > 0)
        {
            blackImageAlpha -= Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, blackImageAlpha);
            // yield return null;
            await Task.Yield();
        }
        blackImage.enabled = false;
    }

    public async void BlackImageFadeIn()
    {
        blackImage.enabled = true;
        blackImageAlpha = 0;
        while (blackImageAlpha < 1)
        {
            blackImageAlpha += Time.deltaTime;
            blackImage.color = new Color(0, 0, 0, blackImageAlpha);
            // yield return null;
            await Task.Yield();
        }
    }

}
