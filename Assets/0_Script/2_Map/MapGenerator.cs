using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun.Demo.Cockpit;
using System.Linq;
using System;

public class MapGenerator : MonoBehaviour
{

    #region Singleton
    private static MapGenerator instance = null;

    private int currentMapWidth = 0;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    public static MapGenerator Instance
    {
        get
        {
            if (null == instance) return null;
            return instance;
        }
    }
    #endregion

    [Header("Grid Variables")]
    [SerializeField] private GameObject gridPrefab;

    [Space(10)]

    [Header("Map Appear Effect")]
    [SerializeField] private float fallDuration;
    [SerializeField] private float timeBetweenFall;
    [SerializeField] private int camOffY;
    [Space(10)]


    private MapGrid[,] mapGrids;



    // Generate Map by List1D
    // TODO : Create 2DArray_Map to manage BoxControl
    private void GenerateMap(ref List<GridInfo> mapArr, int wh)
    {
        currentMapWidth = wh;
        mapGrids = new MapGrid[wh, wh];

        GridInfo grid;

        for (int i = 0; i < mapArr.Count; i++)
        {
            grid = mapArr[i];
            mapGrids[grid.Pos.y, grid.Pos.x] = Instantiate(gridPrefab, new Vector3(grid.Pos.x, 0, grid.Pos.y) * Constant.GRID_SIZE + new Vector3(0, camOffY, 0), Quaternion.identity, transform).GetComponent<MapGrid>();
            mapGrids[grid.Pos.y, grid.Pos.x].transform.localScale = Vector3.one * Constant.GRID_SIZE;
            mapGrids[grid.Pos.y, grid.Pos.x].InitMapGrid(grid);

        }

        StartCoroutine(GridAppearEff(fallDuration));
        return;
    }

    public void SetGridColor(Vector2Int pos, Color color, float duration = 0.4f)
    {
        // TODO : gradation needed
        mapGrids[pos.x, pos.y].GetComponent<MeshRenderer>().material.DOColor(color, duration);
    }

    public MapGrid GetMapGrid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= mapGrids.GetLength(0) || pos.y >= mapGrids.GetLength(1)) return null;
        return mapGrids[pos.x, pos.y];
    }


    public ColorSet GetGridColor(Vector2Int pos)
    {
        return mapGrids[pos.x, pos.y].Gridinfo.Colorset;
    }

    private IEnumerator GridAppearEff(float duration)
    {
        yield return new WaitForSeconds(1f);

        // Appear Grids
        for (int i = 0; i < mapGrids.GetLength(0); i++)
        {
            for (int j = 0; j < mapGrids.GetLength(1); j++)
            {
                if (mapGrids[i, j] == null) continue;
                mapGrids[i, j].AppearGrid(duration);
                yield return new WaitForSeconds(timeBetweenFall);
            }
        }

        yield return new WaitForSeconds(duration);

        // Appear Box

    }

    public bool CheckMapClear()
    {
        for (int i = 0; i < currentMapWidth; i++)
        {
            for (int j = 0; j < currentMapWidth; j++)
            {
                if (mapGrids[i, j] == null) continue;

                if (mapGrids[i, j].Gridinfo.Colorset.GetColor().Equals(ColorConstants.WHITE)) continue;

                return false;
            }
        }

        return true;
    }

    public void TestInit(int n)
    {

        List<GridInfo> mapArrs = new List<GridInfo>();

        MapInfo mapResource = Managers.Resource.GetMapInfo(n);

        foreach (GridInfoEx gi in mapResource.gridInfo) {
            mapArrs.Add(new GridInfo(gi.pos, gi.height, gi.colorIdx, gi.state));
        }

        GenerateMap(ref mapArrs, mapResource.width);
    }

    public void ResetAndInit(int n)
    {
        // TODO : Disappear coroutine needed

        for (int i = 0; i < currentMapWidth; i++)
        {
            for(int j=0; j<currentMapWidth; j++)
            {
                Destroy(mapGrids[i, j].gameObject);
            }
        }
        
        TestInit(n);
    }

}
