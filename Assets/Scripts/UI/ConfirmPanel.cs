using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmPanel : BasePanel
{

    public Button confirmBtn;
    public Button cancelBtn;
    public override void Init()
    {

        confirmBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ConfirmPanel>();
            UIManager.Instance.HidePanel<GamePanel>();
            SceneManager.LoadScene("UIScene");
        });
        cancelBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.HidePanel<ConfirmPanel>();
        });

    }

}
