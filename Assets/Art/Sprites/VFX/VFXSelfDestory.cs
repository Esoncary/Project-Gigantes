using UnityEngine;

public class VFXSelfDestruct : MonoBehaviour
{
    void Start()
    {
        // 获取动画长度
        float duration = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        // 播完就销毁
        Destroy(gameObject, duration);
    }
}