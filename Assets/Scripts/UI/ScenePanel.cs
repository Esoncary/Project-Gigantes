using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenePanel : BasePanel
{
    public Button leftBtn;
    public Button rightBtn;
    public Button beginBtn;
    public Button bakcBtn;
    public Image image;
    public Text name;
    public Text describe;
    private int nowIndex = 0;

    public override void Init()
    {
        GetCurrentSceneData();
        leftBtn.onClick.AddListener(() =>
         {
             print("left");
             --nowIndex;
             if (nowIndex < 0)
                 nowIndex = GameDataMgr.Instance.sceneDatas.Count;
             GetCurrentSceneData();
         });
        rightBtn.onClick.AddListener(() =>
        {
            print("right");
            ++nowIndex;
            if (nowIndex >= GameDataMgr.Instance.sceneDatas.Count)
                nowIndex = 0;
            GetCurrentSceneData();
        });
        beginBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ScenePanel>();
            SceneMgr.Instance.LoadScene(nowIndex);
        });
        bakcBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ScenePanel>();
            UIManager.Instance.ShowPanel<ChoosePanel>();
        });
    }
    public void GetCurrentSceneData()
    {
        SceneData sceneInfo = SceneMgr.Instance.GetSceneData(nowIndex);
        name.text = sceneInfo.name;
        image.sprite = Resources.Load<Sprite>(sceneInfo.imgRes);
    }
}