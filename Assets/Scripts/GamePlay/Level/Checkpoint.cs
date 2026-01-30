using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        isActivated = true;
        // 调用 SceneMgr 更新当前的重生点坐标
        SceneMgr.Instance.UpdateCheckpoint(transform.position);

        // 可选：播放音效或改变动画
        Debug.Log("存档点已激活: " + transform.position);
        // SoundMgr.Instance.PlaySound("SavePoint"); 
    }
}