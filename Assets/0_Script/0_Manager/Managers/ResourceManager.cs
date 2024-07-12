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
    private string diceTexturesPath = "Texture";

    // json->���ҽ� �޾ƿ���, �迭�� ����
    private MapInfos mapInfos = new MapInfos();
    private Texture2D[] diceTextures;

    public void Init()
    {
        mapInfos = JsonUtility.FromJson<MapInfos>(Resources.Load<TextAsset>(mapInfoPath).text);

        diceTextures =  Resources.LoadAll<Texture2D>(diceTexturesPath);

        
    }

    public MapInfo GetMapInfo(int idx)
    {
        return mapInfos.mapInfo[idx];
    }

    public int GetMapCount()
    {
        return mapInfos.mapInfo.Length;
    }

    public Texture2D GetDiceTexture(int idx)
    {
        return diceTextures[idx];   
    }
}