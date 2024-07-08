using DG.Tweening;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : NetworkBehaviour
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

    private void Start()
    {
        isMapMaking = false;
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
    
    [SerializeField] private GameSceneUI gameSceneUI;

    [Header("Skybox Materials")]
    [SerializeField] private List<Material> skyboxMaterials;

    private Coroutine gridAppearCoroutine = null;
    private BoxController curBoxController = null;


    //public MapGrid[,] NetworkedMapGrids { get; set; }
    private bool isMapMaking;

    [UnitySerializeField, Networked, Capacity(50)]
    public NetworkDictionary<Vector2Int, MapGrid> NetworkedMapGrids => default;

    [Networked]
    public int NetworkedCurMapWidth { get; set; }

    public void SetGridColor(Vector2Int pos, Color color, float duration = 0.4f)
    {

        NetworkedMapGrids[pos].GetComponent<MeshRenderer>().material.DOColor(color, duration);
    }

    public MapGrid GetMapGrid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= NetworkedCurMapWidth || pos.y >= NetworkedCurMapWidth) return null;
        return NetworkedMapGrids[pos];
    }


    public ColorSet GetGridColor(Vector2Int pos)
    {
        return NetworkedMapGrids[pos].NetworkedGridInfo.colorset;
    }

    public bool CheckMapClear()
    {
        for (int i = 0; i < NetworkedCurMapWidth; i++)
        {
            for (int j = 0; j < NetworkedCurMapWidth; j++)
            {
                if (NetworkedMapGrids[new Vector2Int(i, j)] == null) continue;

                if (NetworkedMapGrids[new Vector2Int(i, j)].NetworkedGridInfo.colorset.GetColor().Equals(ColorConstants.WHITE)) continue;

                return false;
            }
        }

        return true;
    }

    public void GenerateMap(int n, NetworkRunner runner)
    {
        gameSceneUI.UpdateStageText(n);
        if (n == 1)
            RenderSettings.skybox = skyboxMaterials[1];
        else RenderSettings.skybox = skyboxMaterials[0];

        List<NetworkGridInfo> mapArrs = new List<NetworkGridInfo>();

        MapInfo mapResource = Managers.Resource.GetMapInfo(n);


        foreach (GridInfoEx gi in mapResource.gridInfo) {
            mapArrs.Add(new NetworkGridInfo(gi.pos, gi.height, gi.colorIdx, gi.state));
        }



        NetworkGridInfo grid;

        for (int i = 0; i < mapArrs.Count; i++)
        {
            grid = mapArrs[i];
            NetworkedMapGrids.Set(new Vector2Int(grid.Pos.y, grid.Pos.x), runner.Spawn(gridPrefab, new Vector3(grid.Pos.x, 0, grid.Pos.y) * Constant.GRID_SIZE, Quaternion.identity).GetComponent<MapGrid>());
            NetworkedMapGrids[new Vector2Int(grid.Pos.y, grid.Pos.x)].transform.localScale = Vector3.one * Constant.GRID_SIZE;
            NetworkedMapGrids[new Vector2Int(grid.Pos.y, grid.Pos.x)].InitMapGrid(grid);

        }

        NetworkedCurMapWidth = n;
        //curBoxController = Instantiate(boxPrefab).GetComponent<BoxController>();

        // TODO : pos, height modify needed
        //curBoxController.SetBoxController(new Vector2Int(0, 0), 0);
        return;
    }


    public void EraseAllObject()
    {
        if (isMapMaking) StopCoroutine(gridAppearCoroutine);

        if (curBoxController != null)
            Destroy(curBoxController.gameObject);
        // Disappear Grids
        for (int i = 0; i < NetworkedCurMapWidth; i++)
        {
            for (int j = 0; j < NetworkedCurMapWidth; j++)
            {
                if (NetworkedMapGrids.TryGet(new Vector2Int(i, j), out var value)){
                    Destroy(value.gameObject);
                }
            }
        }
    }


}
