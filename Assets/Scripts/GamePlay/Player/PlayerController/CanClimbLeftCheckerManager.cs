using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanClimbLeftCheckerManager : MonoBehaviour
{
    public bool canClimb;
    public bool isOnLeftWall;
    public LayerMask Ground;
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
            isOnLeftWall = true;
            canClimb = true;
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & Ground) != 0)
        {
            isOnLeftWall = false;
            canClimb = false;
        }
            
    }
}
