// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class ChoosePanel : BasePanel
// {
//     public Button beginBtn;
//     public Button backBtn;
//     public Button leftBtn;
//     public Button rightBtn;
//     public Text characterName;
//     public Text unlockMoney;
//     public Text playerMoney;
//     private int nowIndex = 0;
//     private Transform RolePos;
//     public RoleData nowData;
//     private GameObject roleObj;
//     public Button UnlockBtn;
//     public override void Init()
//     {
//         RolePos = GameObject.Find("RolePos").transform;
//         //加载角色的模型和名字
//         LoadRole();
//         // playerMoney.text = GameDataMgr.Instance.playerInfo.haveMoney.ToString();
//         // characterName.text = GameDataMgr.Instance.
//         //初始化控件
//         beginBtn.onClick.AddListener(() =>
//         {
//             // GameDataMgr.Instance.nowRoleIndex = nowIndex;
//             //进入游戏
//             UIManager.Instance.ShowPanel<ScenePanel>();
//             UIManager.Instance.HidePanel<ChoosePanel>();
//             // SceneManager.LoadScene("GameScene");
//         });
//         backBtn.onClick.AddListener(() =>
//         {
//             UIManager.Instance.HidePanel<ChoosePanel>();
//             // Camera.main.GetComponent<CameraAnimator>().TurnRight(() =>
//             // {
//             //     UIManager.Instance.ShowPanel<BeginPanel>();

//             // });
//         });
//         leftBtn.onClick.AddListener(() =>
//         {
//             --nowIndex;
//             if (nowIndex < 0)
//                 nowIndex = GameDataMgr.Instance.list_RoleData.Count;
//             LoadRole();
//         });
//         rightBtn.onClick.AddListener(() =>
//         {
//             ++nowIndex;
//             if (nowIndex >= GameDataMgr.Instance.list_RoleData.Count)
//                 nowIndex = 0;
//             LoadRole();
//         });
//         UnlockBtn.onClick.AddListener(() =>
//         {
//             // if (GameDataMgr.Instance.playerInfo.haveMoney >= nowData.lockMoney)
//             // {
//             //     //解锁成功
//             //     GameDataMgr.Instance.playerInfo.haveRole.Add(nowData.id);
//             //     GameDataMgr.Instance.playerInfo.haveMoney -= nowData.lockMoney;
//             //     playerMoney.text = GameDataMgr.Instance.playerInfo.haveMoney.ToString();
//             //     UodateLockBtn();
//             //     GameDataMgr.Instance.SavePlayerData();
//             //     //提示面板
//             //     //tip
//             //     UIManager.Instance.ShowPanel<TipPanel>().ChangeText("购买成功");
//             // }
//             // else
//             // {
//             //     //显示弹窗
//             //     UIManager.Instance.ShowPanel<TipPanel>().ChangeText("金额不足");
//             // }
//         });
//     }
//     public void LoadRole()
//     {
//         if (roleObj != null)
//         {
//             Destroy(roleObj);
//             roleObj = null;
//         }
//         nowData = GameDataMgr.Instance.list_RoleData[nowIndex];
//         roleObj = Instantiate(Resources.Load<GameObject>(nowData.res), RolePos.position, RolePos.rotation);
//         // characterName.text = nowData.info.ToString();
//         // Destroy(roleObj.GetComponent<PlayerObj>());
//         UodateLockBtn();
//     }
//     public void UodateLockBtn()
//     {
//         // if (nowData.lockMoney > 0 && !GameDataMgr.Instance.playerInfo.haveRole.Contains(nowData.id))
//         // {
//         //     UnlockBtn.gameObject.SetActive(true);
//         //     unlockMoney.text = nowData.lockMoney.ToString();
//         //     beginBtn.gameObject.SetActive(false);
//         // }
//         // else
//         // {
//         //     UnlockBtn.gameObject.SetActive(false);
//         //     beginBtn.gameObject.SetActive(true);
//         // }
//     }
//     public override void HideMe(UnityAction callBack)
//     {
//         base.HideMe(callBack);
//         if (roleObj != null)
//         {
//             Destroy(roleObj);
//             roleObj = null;
//         }
//     }
// }
