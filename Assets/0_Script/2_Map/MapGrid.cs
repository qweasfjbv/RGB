using UnityEngine;
using DG.Tweening;

public class GridInfo {

    private Vector2Int pos;
    private int height;
    private GridState state;

    public Vector2Int Pos { get { return pos; } }
    public int Height { get { return height; } }
    public GridState State { get { return state; } }

    public GridInfo(Vector2Int pos, int height, GridState state = GridState.NONE)
    {
        this.pos = pos;
        this.height = height;
        this.state = state;
    }
}

public class MapGrid : MonoBehaviour
{
    private GridInfo gridinfo;
    public GridInfo Gridinfo { get { return gridinfo; } }   


    // Process Grid According to GridState
    public void InitMapGrid(GridInfo info)
    {
        gridinfo = new GridInfo(info.Pos, info.Height, info.State);

        switch (info.State)
        {
            case GridState.NONE: return;
            case GridState.START:
                GetComponent<Renderer>().material.color = Color.red;
                break;

            case GridState.END:
                GetComponent<Renderer>().material.color = Color.blue;
                break;

            case GridState.CAMERA:
                Camera.main.GetComponent<CameraController>().SetQuaterView(transform.position - new Vector3(0, transform.position.y, 0));
                break;
        }
    }

    public void AppearGrid(float duration = 1f)
    {
        transform.DOMoveY(gridinfo.Height, duration).SetEase(Ease.InOutElastic);
    }
}
