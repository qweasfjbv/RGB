using JetBrains.Annotations;
using System;
using UnityEngine;


[Serializable]
public class MapInfos
{
    public MapInfo[] mapInfo;
}

[Serializable]
public class MapInfo
{
    public int mapId;
    public int width;
    public GridInfoEx[] gridInfo;
}

[Serializable]
public class GridInfoEx
{
    public Vector2Int pos;
    public int height;
    public GridState state;
    public int colorIdx;
}

public class ResourceManager
{

    private string tutorialMapInfoPath = "JsonData/TutoMapData";
    private string stageMapInfoPath = "JsonData/StageMapData";
    private string multiMapInfoPath = "JsonData/MultiMapData";
    private string diceTexturesPath = "Texture";

    // json->리소스 받아오기, 배열에 저장
    private MapInfos stageMapInfos = new MapInfos();
    private MapInfos tutoMapInfos = new MapInfos();
    private MapInfos multiMapInfos = new MapInfos();
    private Texture2D[] diceTextures;

    public void Init()
    {
        stageMapInfos = JsonUtility.FromJson<MapInfos>(Resources.Load<TextAsset>(stageMapInfoPath).text);
        tutoMapInfos = JsonUtility.FromJson<MapInfos>(Resources.Load<TextAsset>(tutorialMapInfoPath).text);
        multiMapInfos = JsonUtility.FromJson<MapInfos>(Resources.Load<TextAsset>(multiMapInfoPath).text);

        diceTextures =  Resources.LoadAll<Texture2D>(diceTexturesPath);

        
    }

    public MapInfo GetMapInfo(GameType type, int idx)
    {
        switch (type)
        {
            case GameType.TUTO:
                return tutoMapInfos.mapInfo[idx];
            case GameType.STAGE:
                return stageMapInfos.mapInfo[idx];
            case GameType.MULTI:
                return multiMapInfos.mapInfo[idx];
        }

        return null;
    }

    public int GetMapCount(GameType type)
    {
        switch (type) {
            case GameType.TUTO:
                return tutoMapInfos.mapInfo.Length;
            case GameType.STAGE:
                return stageMapInfos.mapInfo.Length;
            case GameType.MULTI:
                return multiMapInfos.mapInfo.Length;
        }
        return -1;
    }

    public Texture2D GetDiceTexture(int idx)
    {
        return diceTextures[idx];   
    }
}