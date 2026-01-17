using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MusicData
{
    public bool musicOpen;
    public bool effectOpen;
    public float musicValue;
    public float effectValue;
    public MusicData()
    {
        musicOpen = true;
        effectOpen = true;
        musicValue = 1;
        effectValue = 1;
    }
}
