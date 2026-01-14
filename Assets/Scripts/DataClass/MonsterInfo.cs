using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MonsterInfo
{
    public int id;
    public string resName;
    public string animator;
    public int atk;
    public int moveSpeed;
    public int roundSpeed;
    public int hp;
    public float atkCD;
}