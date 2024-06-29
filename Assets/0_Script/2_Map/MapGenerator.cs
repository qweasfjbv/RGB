using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

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

    [Header("Prefabs")]
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject boxPrefab;

    [Space(10)]

    [Header("Map Appear Effect")]
    [SerializeField] private float fallDuration;
    [SerializeField] private float timeBetweenFall;
    [SerializeField] private int camOffY;
    [Space(10)]

    private BoxController curBoxController = null;
    private MapGrid[,] mapGrids;

    int nn = 1;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetAndInit(nn);
            nn++;
            nn %= 2;
        }
    }


    public void SetGridColor(Vector2Int pos, Color color, float duration = 0.4f)
    {
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


        curBoxController = Instantiate(boxPrefab).GetComponent<BoxController>();

        // TODO : pos, height modify needed
        curBoxController.SetBoxController(new Vector2Int(0, 0), 0);
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

    public void GenerateMap(int n)
    {

        List<GridInfo> mapArrs = new List<GridInfo>();

        MapInfo mapResource = Managers.Resource.GetMapInfo(n);

        foreach (GridInfoEx gi in mapResource.gridInfo) {
            mapArrs.Add(new GridInfo(gi.pos, gi.height, gi.colorIdx, gi.state));
        }


        currentMapWidth = mapResource.width;
        mapGrids = new MapGrid[currentMapWidth, currentMapWidth];

        GridInfo grid;

        for (int i = 0; i < mapArrs.Count; i++)
        {
            grid = mapArrs[i];
            mapGrids[grid.Pos.y, grid.Pos.x] = Instantiate(gridPrefab, new Vector3(grid.Pos.x, 0, grid.Pos.y) * Constant.GRID_SIZE + new Vector3(0, camOffY, 0), Quaternion.identity, transform).GetComponent<MapGrid>();
            mapGrids[grid.Pos.y, grid.Pos.x].transform.localScale = Vector3.one * Constant.GRID_SIZE;
            mapGrids[grid.Pos.y, grid.Pos.x].InitMapGrid(grid);

        }

        StartCoroutine(GridAppearEff(fallDuration));
        return;
    }

    // Restart or NextLevel in GameScene
    public void ResetAndInit(int n)
    {
        StartCoroutine(GridDisappearEff(fallDuration, n));

    }

    private IEnumerator GridDisappearEff(float duration, int n)
    {
        curBoxController.UnsetBoxController();
        curBoxController = null;

        yield return new WaitForSeconds(0.5f);

        // Disappear Grids
        for (int i = 0; i < mapGrids.GetLength(0); i++)
        {
            for (int j = 0; j < mapGrids.GetLength(1); j++)
            {
                if (mapGrids[i, j] == null) continue;
                mapGrids[i, j].DisappearGrid(duration);
                yield return new WaitForSeconds(timeBetweenFall);
            }
        }

        yield return new WaitForSeconds(duration);

        GenerateMap(n);

    }

}
