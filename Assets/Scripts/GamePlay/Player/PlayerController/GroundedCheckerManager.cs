using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheckerManager : MonoBehaviour
{
    public bool isGrounded;
    public LayerMask Ground;
    public PlayerVFXController PlayerVFXController;

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
            isGrounded = true;
            PlayerVFXController.PlayLandDust(this.transform);
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & Ground) != 0)
        {
            isGrounded = false;
        }
            
    }
}
