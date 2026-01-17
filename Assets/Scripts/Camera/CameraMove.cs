using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Vector3 offsetPos = new Vector3(0, 5, -5);
    public float bodyHeight = 2;
    public Transform target;
    public float moveSpeed = 10;
    public float rotateSpeed = 10;
    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;
        Vector3 targetPos = target.position + target.forward * offsetPos.z;
        targetPos += Vector3.up * offsetPos.y;
        targetPos += target.right * offsetPos.x;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);

        Quaternion quaternion = Quaternion.LookRotation(target.position + bodyHeight * Vector3.up - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, quaternion, rotateSpeed * Time.deltaTime);
    }
    public void SetTarget(Transform player)
    {
        target = player;
    }
}
