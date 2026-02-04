using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheckerManager : MonoBehaviour
{
    public bool isGrounded => groundContactCount > 0;
    public LayerMask Ground;
    public PlayerVFXController PlayerVFXController;

    private int groundContactCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & Ground) != 0)
        {
            groundContactCount++;
            //PlayerVFXController.PlayLandDust(this.transform);//不播放落地特效了，不然会非常杂乱
            // SoundEffectMgr.Instance.PlayFootstep(collision.gameObject.tag);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & Ground) != 0)
        {
            groundContactCount = Mathf.Max(0, groundContactCount - 1);
        }

    }
}
