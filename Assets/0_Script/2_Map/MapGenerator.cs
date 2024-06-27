using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq.Expressions;
using Unity.VisualScripting;
using System.ComponentModel.Design.Serialization;

public class MapGenerator : MonoBehaviour
{

    #region Singleton
    private static MapGenerator instance = null;

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

    private void Start()
    {
        TestInit();
    }

    // Generate Map by List1D
    // TODO : Create 2DArray_Map to manage BoxControl
    private void GenerateMap(ref List<GridInfo> mapArr, int wh)
    {
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



    // Test Init
    private void TestInit()
    {
        List<GridInfo> mapArrs = new List<GridInfo>();
        mapArrs.Add(new GridInfo(new Vector2Int(0, 0), 0, 7, 1));
        mapArrs.Add(new GridInfo(new Vector2Int(0, 1), 0, 1));
        mapArrs.Add(new GridInfo(new Vector2Int(0, 2), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(0, 3), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(0, 4), 0, 2));
        mapArrs.Add(new GridInfo(new Vector2Int(1, 0), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(1, 1), 0, 3));
        mapArrs.Add(new GridInfo(new Vector2Int(1, 2), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(1, 3), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(1, 4), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(2, 1), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(2, 2), 0, 7, 2));
        mapArrs.Add(new GridInfo(new Vector2Int(2, 3), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(2, 4), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(3, 0), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(3, 1), 0, 1));
        mapArrs.Add(new GridInfo(new Vector2Int(3, 2), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(3, 3), 0, 0));
        mapArrs.Add(new GridInfo(new Vector2Int(3, 4), 0, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(4, 0), 1, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(4, 1), 1, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(4, 2), 1, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(4, 3), 1, 7));
        mapArrs.Add(new GridInfo(new Vector2Int(4, 4), 1, 7));

        GenerateMap(ref mapArrs, 5);
    }

}
