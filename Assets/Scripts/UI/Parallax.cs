using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam; // 引用主摄像机
    [Tooltip("0 = 跟随相机(近景), 1 = 这种完全不动, 0.5 = 移动一半速度")]
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        // 如果背景需要循环，记录图片的宽度
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // 计算背景应该移动的相对距离
        float distance = (cam.transform.position.x * parallaxEffect);

        // 更新背景位置
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);

        // --- 循环背景逻辑（如果你需要背景无限循环） ---
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        if (temp > startPos + length) startPos += length;
        else if (temp < startPos - length) startPos -= length;
    }
}