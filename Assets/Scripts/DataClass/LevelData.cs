using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
//场景配置
public class LevelData
{
    public int LevelId;
    public string SceneName;
    public string LevelName;
    public string imgRes;
    public int WorldId;// 章节编号
    public int OrderInWorld;// 关卡编号
    public int TotalCollectibles;// 本关可收集总数
}
