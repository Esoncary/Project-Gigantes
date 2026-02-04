using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Cooler : MonoBehaviour, IPickUp
{
    [Header("道具共有参数设置")]
    public float respawnTime = 3.0f;
    public float floatRange = 0.2f;
    public float floatSpeed = 2.0f;

    protected Vector3 startPos;
    protected SpriteRenderer sr;
    protected Collider2D col;
    protected bool isCollected = false;

    public GameObject VFXPickup;
    


    [Header("coller参数设置")]
    public float coolTime;


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
        player.coolerTimer = coolTime;//启动冷冻计时器
        player.playerVFXController.PlayCooled();//启动冷冻特效。这种启动方式有问题，但是先这样吧
        player.playerVFXController.StopOverload();//关闭过载特效。如果角色处于过载状态吃到，关闭过载特效
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
