using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Strengthener : MonoBehaviour, IPickUp
{
    //这个是增强剂，它会短暂地将maxStorage和explosionStorage提高百分之20
    [Header("道具共有参数设置")]
    public float respawnTime = 3.0f;
    public float floatRange = 0.2f;
    public float floatSpeed = 2.0f;

    protected Vector3 startPos;
    protected SpriteRenderer sr;
    protected Collider2D col;
    protected bool isCollected = false;

    public GameObject VFXPickup;

    public float strenthenerTime;//强化持续时间


    private void Awake()
    {
        startPos = transform.position;
        sr = GetComponentInChildren<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //空中浮动逻辑
        if (!isCollected)
        {
            float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatRange;
            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
    }

    void IPickUp.PickUpEffect(PlayerController player)
    {
        //如果已经被捡起来，什么也不做
        if (isCollected) return;

        //如果玩家刚捡起来
        player.strengthenerTimer = strenthenerTime;//启动强化计时器
        player.maxStorage += 35;//提高最大储量
        player.explosionStorageThrehold += 35;//提高爆炸储量阈值

        if (player.rb.velocity.magnitude > 0.001f) //拾取增强剂时瞬间提高速度
        {
            // 1. 获取当前运动方向 (长度为1的单位向量)
            Vector2 direction = player.rb.velocity.normalized;

            // 2. 获取当前的模长 (速度值)
            float currentSpeed = player.rb.velocity.magnitude;

            // 3. 重新赋值：方向 * (旧模长 + 增加值)
            player.rb.velocity = direction * (currentSpeed + 15f);//15是提升的速度值
        }

        player.playerVFXController.PlayStrengthened();//播放增强剂特效
        Instantiate(VFXPickup, transform.position, Quaternion.identity);//播放拾取特效

        //启动刷新协程
        StartCoroutine(RespawnSequence());
    }


    private IEnumerator RespawnSequence()
    {
        isCollected = true;
        sr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        // 重刷
        isCollected = false;
        transform.position = startPos;
        sr.enabled = true;
        col.enabled = true;
    }
}
