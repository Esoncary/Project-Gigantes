using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraController : MonoBehaviour
{
    #region 单例化
    public static VirtualCameraController Instance { get; private set; }

    private CinemachineConfiner2D confiner;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //获取组件引用
        vcam = GetComponent<CinemachineVirtualCamera>();
    }
    #endregion

    CinemachineVirtualCamera vcam;

    // Start is called before the first frame update
    void Start()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (confiner.m_BoundingShape2D == null)
        {
            ResetCameraBound();//绑定相机边界
        }
       
    }

    public void ResetCameraTarget(Transform playerTransform)
    {
        // Debug.Log("state: ");
        if (vcam != null)
        {
            vcam.Follow = playerTransform;
            // 2. 强制提升优先级，确保新场景的 Brain 能立刻切换到这个相机
            // 先降后升，触发内部刷新
            vcam.Priority = 0;
            vcam.Priority = 10;

            // vcam.LookAt = playerTransform; // 2D 游戏通常只需要 Follow，不需要 LookAt
            Debug.Log("相机已成功绑定新角色！");
        }
    }

    public void ResetCameraBound()//重新寻找相机边界
    {
        GameObject cameraBound = GameObject.Find("CameraBound");
        Collider2D cameraBoundCol;
        if (cameraBound != null)
        {
            cameraBoundCol = cameraBound.GetComponent<Collider2D>();
            if (cameraBoundCol != null)
            {
                confiner.m_BoundingShape2D = cameraBoundCol;
                confiner.InvalidateCache();
            }
        }
        
    }
}
