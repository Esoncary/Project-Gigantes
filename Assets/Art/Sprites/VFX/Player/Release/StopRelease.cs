using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopVFXRelease : MonoBehaviour
{
    public SpriteRenderer VFXReleaseRenderer;
    
    
    // Start is called before the first frame update
    void Start()
    {
        VFXReleaseRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopRelease()
    {
        if (VFXReleaseRenderer != null) VFXReleaseRenderer.enabled = false;
    }
}
