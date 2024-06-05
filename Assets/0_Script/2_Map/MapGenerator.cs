using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;


public class MapGenerator : MonoBehaviour
{

    #region Singleton
    private static MapGenerator instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
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

    [SerializeField] private GameObject gridPrefab;

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
        MapGrid center= null;

        for (int i = 0; i < wh; i++)
        {
            for(int j=0; j<wh; j++)
            {
                grid = mapArr[i * wh + j];
                mapGrids[i, j] = Instantiate(gridPrefab, new Vector3(grid.Pos.x, camOffY, grid.Pos.y), Quaternion.identity, transform).GetComponent<MapGrid>();
                mapGrids[i, j].InitMapGrid(grid);

                if (grid.State == GridState.CAMERA) center = mapGrids[i, j];    
            }
        }

        StartCoroutine(GridAppearEff(fallDuration));
        return;
    }


    private IEnumerator GridAppearEff(float duration)
    {
        yield return new WaitForSeconds(1f);

        // Appear Grids
        for (int i = 0; i < mapGrids.GetLength(0); i++)
        {
            for (int j = 0; j < mapGrids.GetLength(1); j++)
            {
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
        mapArrs.Add(new GridInfo(new Vector2Int(0, 0), 0, GridState.START));
        mapArrs.Add(new GridInfo(new Vector2Int(0, 1), 0));
        mapArrs.Add(new GridInfo(new Vector2Int(0, 2), 0));
        mapArrs.Add(new GridInfo(new Vector2Int(1, 0), 0));
        mapArrs.Add(new GridInfo(new Vector2Int(1, 1), 0, GridState.CAMERA));
        mapArrs.Add(new GridInfo(new Vector2Int(1, 2), 0));
        mapArrs.Add(new GridInfo(new Vector2Int(2, 0), 1));
        mapArrs.Add(new GridInfo(new Vector2Int(2, 1), 1));
        mapArrs.Add(new GridInfo(new Vector2Int(2, 2), 1, GridState.END));

        // TODO : Get Mapinfo from server
        GenerateMap(ref mapArrs, 3);
    }

}
