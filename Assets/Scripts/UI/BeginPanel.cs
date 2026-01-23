using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Button startBtn;
    public Button settingBtn;
    public Button quitBtn;
    public override void Init()
    {
        // 开始按钮
        startBtn.onClick.AddListener(() =>
        {
            //进入选角界面
            UIManager.Instance.HidePanel<BeginPanel>();
            //Camera.main.GetComponent<CameraAnimator>().TurnLeft(() =>
            //{
            //    //显示关卡选择面板
            //    UIManager.Instance.ShowPanel<ScenePanel>();
            //});
        });

        // 设置按钮
        settingBtn.onClick.AddListener(() =>
        {
            //进入设置界面
            UIManager.Instance.ShowPanel<SettingPanel>();
        });

        // 退出按钮
        quitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
