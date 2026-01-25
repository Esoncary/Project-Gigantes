// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class UIMgr : MonoBehaviour
// {
//     #region 单例化
//     public static UIMgr Instance { get; private set; }
//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }
//     #endregion

//     [Header("各种UI组件")]
//     public Image blackImage;//用来淡入和淡出的黑色图片
//     public float blackImageAlpha;//黑幕的alpha值

//     // Start is called before the first frame update
//     void Start()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {

//     }

//     //黑屏淡入和淡出函数与协程
//     public void StartBlackImageFadeIn()
//     {
//         StartCoroutine(BlackImageFadeIn());
//     }
//     public void StartBlackImageFadeOut()
//     {
//         StartCoroutine(BlackImageFadeOut());
//     }

//     public IEnumerator BlackImageFadeOut()
//     {
//         blackImageAlpha = 1;
//         while (blackImageAlpha > 0)
//         {
//             blackImageAlpha -= Time.deltaTime;
//             blackImage.color = new Color(0, 0, 0, blackImageAlpha);
//             yield return null;
//         }
//         blackImage.enabled = false;
//     }

//     public IEnumerator BlackImageFadeIn()
//     {
//         blackImage.enabled = true;
//         blackImageAlpha = 0;
//         while (blackImageAlpha < 1)
//         {
//             blackImageAlpha += Time.deltaTime;
//             blackImage.color = new Color(0, 0, 0, blackImageAlpha);
//             yield return null;
//         }
//     }

// }
