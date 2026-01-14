using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataMgr
{
    //音乐数据
    public MusicData musicDatas;

    //角色数据
    public List<RoleData> roleInfos;

    //场景
    public LinkedList<SceneData> sceneDatas;

    //怪物数据
    // public List<MonsterInfo> monsterInfos;


    private static GameDataMgr instance = new GameDataMgr();

    public static GameDataMgr Instance => instance;

    public GameDataMgr()
    {
        musicDatas = JsonMgr.Instance.LoadData<MusicData>("MusicData");
        roleInfos = JsonMgr.Instance.LoadData<List<RoleData>>("RoleInfo");
        sceneDatas = JsonMgr.Instance.LoadData<LinkedList<SceneData>>("SceneInfo");
        // monsterInfos = JsonMgr.Instance.LoadData<List<MonsterInfo>>("MonsterInfo");
    }
    //存储音乐数据
    public void SaveMusicData()
    {
        JsonMgr.Instance.SaveData(musicDatas, "MusicDatas");
    }
    //存储场景数据
    public void SaveSceneData()
    {
        JsonMgr.Instance.SaveData(sceneDatas, "SceneInfo");
    }
}
