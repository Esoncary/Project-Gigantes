using System;
using UnityEngine;

[Serializable]
public class MachineSaveData
{
    public string machineId;
    // 新增：父物体ID（用于判断层级关系）
    public string parentMachineId;
    // 世界坐标（父物体用）
    public float worldPosX;
    public float worldPosY;
    public float worldPosZ;
    public float worldRotX;
    public float worldRotY;
    public float worldRotZ;
    // 局部坐标（子物体用）
    public float localPosX;
    public float localPosY;
    public float localPosZ;
    public float localRotX;
    public float localRotY;
    public float localRotZ;

    public MachineSaveData() { }

    // 构造函数：自动区分父/子并存储对应坐标
    public MachineSaveData(MachineIdentity identity)
    {
        machineId = identity.machineUniqueId;
        Transform transform = identity.transform;

        // 如果有父物体且父物体也带MachineIdentity，记录父ID并存储局部坐标
        if (transform.parent != null && transform.parent.TryGetComponent(out MachineIdentity parentIdentity))
        {
            parentMachineId = parentIdentity.machineUniqueId;
            // 子物体存局部坐标
            localPosX = transform.localPosition.x;
            localPosY = transform.localPosition.y;
            localPosZ = transform.localPosition.z;
            localRotX = transform.localRotation.eulerAngles.x;
            localRotY = transform.localRotation.eulerAngles.y;
            localRotZ = transform.localRotation.eulerAngles.z;
        }
        else
        {
            parentMachineId = ""; // 标记为根物体
            // 父物体存世界坐标
            worldPosX = transform.position.x;
            worldPosY = transform.position.y;
            worldPosZ = transform.position.z;
            worldRotX = transform.rotation.eulerAngles.x;
            worldRotY = transform.rotation.eulerAngles.y;
            worldRotZ = transform.rotation.eulerAngles.z;
        }
    }
}