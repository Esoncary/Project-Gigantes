using UnityEngine;

public class test1 : MonoBehaviour
{
    private static bool _hasCalledLoadGameScene = false;

    public void Start()
    {
        if (_hasCalledLoadGameScene)
        {
            Debug.Log("LoadGameScene 已全局调用过，本次跳过");
            return;
        }

        GameDataMgr.Instance.LoadChoosePlayerSaveData(0);
        SceneMgr.Instance.LoadGameScene(0);

        _hasCalledLoadGameScene = true;
    }

    public void ResetGlobalLoadFlag()
    {
        _hasCalledLoadGameScene = false;
    }

    private void OnApplicationQuit()
    {
        _hasCalledLoadGameScene = false;
    }
}