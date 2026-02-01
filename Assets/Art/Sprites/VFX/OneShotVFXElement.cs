using UnityEngine;
public class AutoDestroy : MonoBehaviour
{
    
    //此脚本挂在那些一次性特效上，用于销毁它们
    
    public void DestroyVFX()
    {
        Destroy(gameObject);
    }
}