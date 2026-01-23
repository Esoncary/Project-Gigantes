using UnityEngine;
using TMPro; // 如果用的是普通的 Text，改为 using UnityEngine.UI;

//这个脚本用来在UI上显示玩家的速度信息，方便调试

public class VelocityDisplay : MonoBehaviour
{
    private TextMeshProUGUI textComponent; // 如果是普通 Text，改为 Text
    private PlayerController player;

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // 解决你之前的引用丢失问题：如果当前没有玩家引用，去 LevelMgr 拿最新的
        if (player == null)
        {
            if (LevelMgr.Instance != null && LevelMgr.Instance.player != null)
            {
                player = LevelMgr.Instance.player;
            }
            return; // 这一帧先不跑，等拿到玩家再说
        }

        // 获取速度并格式化显示
        // "F2" 表示保留两位小数
        Vector2 v = player.rb.velocity;
        textComponent.text = $"速度: ({v.x:F2}, {v.y:F2})\n合速度: {v.magnitude:F2}";
    }
}