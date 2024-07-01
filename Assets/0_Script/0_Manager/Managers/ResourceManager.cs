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

    private string mapInfoPath = "JsonData/MapData";

    // json->리소스 받아오기, 배열에 저장
    private MapInfos mapInfos = new MapInfos();


    public void Init()
    {
        mapInfos = JsonUtility.FromJson<MapInfos>(Resources.Load<TextAsset>(mapInfoPath).text);
    }

    public MapInfo GetMapInfo(int idx)
    {
        return mapInfos.mapInfo[idx];
    }

    public int GetMapCount()
    {
        return mapInfos.mapInfo.Length;
    }
}