using UnityEngine;

public class OneShotVFXElement : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // 这个函数将作为动画事件(Animation Event)放在动画的最后一帧
    public void FinishVFX()
    {
        sr.enabled = false;
        // Debug.Log(gameObject.name + " 特效播放完毕，已隐藏");
    }
}