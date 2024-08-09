using DG.Tweening;
using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : NetworkBehaviour, ISpawned
{

    #region Singleton

    private static MapGenerator instance = null;

    public static MapGenerator Instance
    {
        get
        {
            if (null == instance) return null;
            return instance;
        }
    }

    
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetSingleton()
    {
        Debug.Log(this.name);
        instance = this;
    }

    #endregion

    [Header("Prefabs")]
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject boxPrefab;

    [Space(10)]

    [Header("Map Appear Effect")]
    [SerializeField] private float timeBetweenFall;
    [SerializeField] private int camOffY;
    [Space(10)]

    [Header("UIs")]
    [SerializeField] private GameSceneUI gameSceneUI;
    [SerializeField] private ResultPanelUI resultPanelUI;
    [SerializeField] private float popupDuration;
    [Space(10)]

    [Header("Skybox Materials")]
    [SerializeField] private List<Material> skyboxMaterials;



    private Coroutine gridAppearCoroutine = null;
    private BoxController curBoxController = null;


    [Networked, Capacity(50)]
    public NetworkDictionary<Vector2Int, MapGrid> NetworkedMapGrids => default;

    [Networked]
    public int NetworkedCurMapWidth { get; set; }


    private bool isSpawned = false;
    public bool IsSpawned { get => isSpawned; }

    public override void Spawned()
    {
        // DO AFTER SPAWN GENERATE

        if (null == instance)
        {
            instance = this;
        }

        isSpawned = true;


        gameSceneUI = FindObjectOfType<GameSceneUI>();
        resultPanelUI = FindObjectOfType<ResultPanelUI>();
        if(resultPanelUI != null) resultPanelUI.gameObject.SetActive(false);
        UpdateMapGrids();
    }

    public void SetGridColor(Vector2Int pos, Color color, float duration = 0.4f)
    {
        if (!HasStateAuthority) return;

        var tGridInfo= NetworkedMapGrids[pos].NetworkedGridInfo;
        var tColorSet = tGridInfo.colorset;
        tColorSet.SetColor(color);
        tGridInfo.colorset = tColorSet;
        NetworkedMapGrids[pos].SetGridInfo(tGridInfo);

    }

    public MapGrid GetMapGrid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= NetworkedCurMapWidth || pos.y >= NetworkedCurMapWidth) return null;
        MapGrid value;
        if (NetworkedMapGrids.TryGet(pos, out value))
        {
            return value;
        }
        else return null;
    }

    public void UpdateMapGrids()
    {
        for (int i = 0; i < NetworkedCurMapWidth; i++)
        {
            for (int j = 0; j < NetworkedCurMapWidth; j++)
            {
                MapGrid tmpGrid;
                NetworkedMapGrids.TryGet(new Vector2Int(i, j), out tmpGrid);
                if (tmpGrid != null)
                {
                    tmpGrid.RPC_UpdateGridVisuals();
                }
            }
        }
    }

    public ColorSet GetGridColor(Vector2Int pos)
    {
        MapGrid tMapGrid = GetMapGrid(pos);
        if (tMapGrid == null)
        {
            Debug.Log("WIDTH : " + NetworkedCurMapWidth);
            for(int i=0; i< NetworkedCurMapWidth; i++)
            {
                for (int j = 0; j < NetworkedCurMapWidth; j++)
                {
                    NetworkedMapGrids.TryGet(pos, out tMapGrid);
                    if (tMapGrid != null) Debug.Log(tMapGrid.NetworkedGridInfo.Pos);
                }
            }
        }

        return NetworkedMapGrids[pos].NetworkedGridInfo.colorset;
    }

    public bool CheckMapClear()
    {
        MapGrid value;
        for (int i = 0; i < NetworkedCurMapWidth; i++)
        {
            for (int j = 0; j < NetworkedCurMapWidth; j++)
            {
                if (!NetworkedMapGrids.TryGet(new Vector2Int(i, j), out value)) continue;

                if (value.NetworkedGridInfo.colorset.GetColor().Equals(ColorConstants.WHITE)) continue;

                return false;
            }
        }

        return true;
    }

    public void SetStageName(GameType type, int n, NetworkRunner runner)
    {
        gameSceneUI.UpdateStageText(type, n, runner);
    }

    public void GenerateMap(GameType type,  int n, NetworkRunner runner)
    {

        if (n == 1)
            RenderSettings.skybox = skyboxMaterials[1];
        else RenderSettings.skybox = skyboxMaterials[0];


        if (type == GameType.TUTO)
        {
            TutorialManager.Instance.PopupHint(Managers.Data.GetBasicSetting().isBlind, n);
        }

        List<NetworkGridInfo> mapArrs = new List<NetworkGridInfo>();

        MapInfo mapResource = Managers.Resource.GetMapInfo(type, n);


        foreach (GridInfoEx gi in mapResource.gridInfo) {
            mapArrs.Add(new NetworkGridInfo(gi.pos, gi.height, gi.colorIdx, gi.state));
        }


        NetworkGridInfo grid;

        NetworkedMapGrids.Clear();
        Debug.Log("GENERTE MAEP : " + mapArrs.Count);
        for (int i = 0; i < mapArrs.Count; i++)
        {
            Debug.Log("SPAWN!");
            grid = mapArrs[i];
            var sp = runner.Spawn(gridPrefab, new Vector3(grid.Pos.x, 0, grid.Pos.y) * Constant.GRID_SIZE, Quaternion.identity,
               inputAuthority: null,
               (runner, NO) => NO.GetComponent<MapGrid>().InitMapGrid(grid));

            NetworkedMapGrids.Set(new Vector2Int(grid.Pos.y, grid.Pos.x), sp.GetComponent<MapGrid>());

        }

        NetworkedCurMapWidth = mapResource.width;
        return;
    }



    public void EraseAllObject(NetworkRunner runner)
    {

        if (curBoxController != null)
            Destroy(curBoxController.gameObject);
        // Disappear Grids
        for (int i = 0; i < NetworkedCurMapWidth; i++)
        {
            for (int j = 0; j < NetworkedCurMapWidth; j++)
            {
                if (NetworkedMapGrids.TryGet(new Vector2Int(i, j), out var value)){
                    runner.Despawn(value.gameObject. GetComponent<NetworkObject>());
                }
            }
        }
    }

    public void PopupResultPanel()
    {
        resultPanelUI.SetResultPanel(GameManagerEx.Instance.CurGameType, GameManagerEx.Instance.CurLv);
        resultPanelUI.GetComponent<RectTransform>().localScale = Vector3.zero;
        resultPanelUI.gameObject.SetActive(true);
        resultPanelUI.GetComponent<RectTransform>().DOScale(Vector3.one, popupDuration).SetEase(Ease.OutQuart);
    }

}
