using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

public enum JsonType
{
    JsonUtlity,
    LitJson,
}
public class JsonMgr
{
    private static JsonMgr instance = new JsonMgr();
    public static JsonMgr Instance => instance;
    private JsonMgr() { }

    //存储
    public void SaveData(object data, string fileName, JsonType type = JsonType.LitJson)
    {
        string str = "";
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        //序列化
        switch (type)
        {
            case JsonType.LitJson:
                str = JsonMapper.ToJson(data);
                break;
            case JsonType.JsonUtlity:
                str = JsonUtility.ToJson(data);
                break;
        }
        File.WriteAllText(path, str);
    }
    //加载
    public T LoadData<T>(string fileName, JsonType type = JsonType.LitJson) where T : new()
    {
        string path = Application.streamingAssetsPath + "/" + fileName + ".json";
        if (!File.Exists(path))
            path = Application.persistentDataPath + "/" + fileName + ".json";
        if (!File.Exists(path))
            return new T();
        //反序列化
        string str = File.ReadAllText(path);
        T data = default(T);
        switch (type)
        {
            case JsonType.LitJson:
                data = JsonMapper.ToObject<T>(str);
                break;
            case JsonType.JsonUtlity:
                data = JsonUtility.FromJson<T>(str);
                break;
        }
        return data;
    }
}
