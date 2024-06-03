using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public enum GridState { NONE = 0, START, END, CAMERA }

public class MapGrid
{
    private Vector2Int pos;
    private int height;
    private GridState state;

    public Vector2Int Pos {  get { return pos; } }
    public int Height { get { return height; } }
    public GridState State { get { return state; } }

    public MapGrid(Vector2Int pos, int height, GridState state = GridState.NONE)
    {
        this.pos = pos;
        this.height = height;
        this.state = state;
    }
}

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

    private void Start()
    {
        TestInit();
    }

    // Generate Map by List1D
    // TODO : Create 2DArray_Map to manage BoxControl
    private void GenerateMap(ref List<MapGrid> mapArr)
    {

        foreach(var grid in  mapArr)
        {
            var tmpGrid = Instantiate(gridPrefab, new Vector3(grid.Pos.x, grid.Height, grid.Pos.y), Quaternion.identity, transform);
            GridStateProcess(tmpGrid, grid.State);
        }

        return;
    }


    // Process Grid According to GridState
    private void GridStateProcess(GameObject grid, GridState state)
    {
        switch (state) {
            case GridState.NONE: return;
            case GridState.START:
                grid.GetComponent<Renderer>().material.color = Color.red;
                break;

            case GridState.END:
                grid.GetComponent<Renderer>().material.color = Color.blue;
                break;

            case GridState.CAMERA:
                Camera.main.GetComponent<CameraController>().SetQuaterView(new Vector3(0, 0, 0));
                break;
        }
    }


    // Test Init
    private void TestInit()
    {
        List<MapGrid> mapGrids = new List<MapGrid>();
        mapGrids.Add(new MapGrid(new Vector2Int(0, 0), 0, GridState.START));
        mapGrids.Add(new MapGrid(new Vector2Int(0, 1), 0));
        mapGrids.Add(new MapGrid(new Vector2Int(0, 2), 0));
        mapGrids.Add(new MapGrid(new Vector2Int(1, 0), 0));
        mapGrids.Add(new MapGrid(new Vector2Int(1, 1), 0, GridState.CAMERA));
        mapGrids.Add(new MapGrid(new Vector2Int(1, 2), 0));
        mapGrids.Add(new MapGrid(new Vector2Int(2, 0), 1));
        mapGrids.Add(new MapGrid(new Vector2Int(2, 1), 1));
        mapGrids.Add(new MapGrid(new Vector2Int(2, 2), 1, GridState.END));

        GenerateMap(ref mapGrids);
    }

}
