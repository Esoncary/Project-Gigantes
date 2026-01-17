using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    public Slider sliderHp;
    public Text wave;
    public Text money;
    public Text hpText;
    public Button backBtn;
    public Transform botTrans;
    public List<TowerBtn> listTowerBtn;
    public override void Init()
    {

        backBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<GamePanel>();
            SceneManager.LoadScene("BeginScene");
            //场景转换
        });
        // botTrans.gameObject.SetActive(false);
    }
    public void ChangeHp(int hp, int maxHp)
    {
        sliderHp.value = (float)(maxHp - hp) / maxHp;
        hpText.text = hp + "/" + maxHp;
    }
    public void ChangWave(int nowNum, int maxNum)
    {
        wave.text = nowNum + "/" + maxNum;
    }
    public void ChangeMoney(int v)
    {
        money.text = v.ToString();
    }
}
