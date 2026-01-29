using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    public Toggle musicToggle;
    public Toggle effectToggle;
    public Slider musicSlider;
    public Slider effectSlider;
    public Button closeBtn;
    public Button continueGameBtn;
    public Button restartBtn;
    public Button mainMenuBtn;
    public override void Init()
    {
        // 加载数据
        MusicData musicData = GameDataMgr.Instance.musicDatas;
        musicToggle.isOn = musicData.musicOpen;
        effectToggle.isOn = musicData.effectOpen;
        musicSlider.value = musicData.musicValue;
        effectSlider.value = musicData.effectValue;
        // BGM控件
        musicToggle.onValueChanged.AddListener((v) =>
        {
            BkgMusicMgr.Instance.SetIsOpen(v);
            musicData.musicOpen = v;
        });

        musicSlider.onValueChanged.AddListener((v) =>
        {
            BkgMusicMgr.Instance.SetVolume(v);
            musicData.musicValue = v;
        });
        // 音效控件
        effectToggle.onValueChanged.AddListener((v) =>
        {
            SoundEffectMgr.Instance.SetIsOpen(v);
            musicData.effectOpen = v;
        });

        effectSlider.onValueChanged.AddListener((v) =>
        {
            SoundEffectMgr.Instance.SetVolume(v);
            musicData.effectValue = v;
        });
        // 关闭按钮
        closeBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.HidePanel<SettingPanel>();
            // 逻辑处理
            GameDataMgr.Instance.SaveMusicData();
        });
        // 继续游戏按钮
        continueGameBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.HidePanel<SettingPanel>();
            // 逻辑处理
            GameDataMgr.Instance.SaveMusicData();
        });
        // 重新开始按钮
        restartBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.HidePanel<SettingPanel>();
            // 逻辑处理
            GameDataMgr.Instance.SaveMusicData();
            SceneMgr.Instance.TriggerReload();
        });
        // 主菜单按钮
        mainMenuBtn.onClick.AddListener(() =>
        {
            // UI处理
            UIManager.Instance.ShowPanel<ConfirmPanel>();
            // 逻辑处理
            GameDataMgr.Instance.SaveMusicData();
        });
    }
    public override void ShowMe()
    {
        base.ShowMe();
        Time.timeScale = 0;
    }
    public override void HideMe(UnityAction callBack)
    {
        base.HideMe(callBack);
        Time.timeScale = 1;
    }

    public void HideBtn()
    {
        continueGameBtn.gameObject.SetActive(false);
        restartBtn.gameObject.SetActive(false);
        mainMenuBtn.gameObject.SetActive(false);
    }
    public void ShowBtn()
    {
        continueGameBtn.gameObject.SetActive(true);
        restartBtn.gameObject.SetActive(true);
        mainMenuBtn.gameObject.SetActive(true);
    }

}
