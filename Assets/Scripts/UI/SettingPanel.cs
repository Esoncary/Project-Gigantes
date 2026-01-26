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
        //初始时 得到存储的数据
        MusicData musicData = GameDataMgr.Instance.musicDatas;
        musicToggle.isOn = musicData.musicOpen;
        effectToggle.isOn = musicData.effectOpen;
        musicSlider.value = musicData.musicValue;
        effectSlider.value = musicData.effectValue;
        //初始化控件
        musicToggle.onValueChanged.AddListener((v) =>
        {
            BkgMusicMgr.Instance.SetIsOpen(v);
            musicData.musicOpen = v;
        });
        effectToggle.onValueChanged.AddListener((v) =>
        {
            //设置音效
            musicData.effectOpen = v;
        });
        musicSlider.onValueChanged.AddListener((v) =>
        {
            BkgMusicMgr.Instance.SetVolume(v);
            musicData.musicValue = v;
        });
        effectSlider.onValueChanged.AddListener((v) =>
        {
            musicData.effectValue = v;
        });
        closeBtn.onClick.AddListener(() =>
        {
            GameDataMgr.Instance.SaveMusicData();
            UIManager.Instance.HidePanel<SettingPanel>();
        });
        continueGameBtn.onClick.AddListener(() =>
        {
            GameDataMgr.Instance.SaveMusicData();
            UIManager.Instance.HidePanel<SettingPanel>();
        });
        restartBtn.onClick.AddListener(() =>
        {
            SceneMgr.Instance.TriggerReload();
            UIManager.Instance.HidePanel<SettingPanel>();
        });
        mainMenuBtn.onClick.AddListener(() =>
        {
            // SceneMgr.Instance.TriggerReload();
            // UIManager.Instance.HidePanel<SettingPanel>();
            UIManager.Instance.ShowPanel<ConfirmPanel>();
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
