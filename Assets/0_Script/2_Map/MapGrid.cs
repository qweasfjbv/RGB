using UnityEngine;
using DG.Tweening;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;

[Serializable]
public class ColorSet
{
    public int r;
    public int g;
    public int b;

    public int Total { get => (r + g + b); }

    public ColorSet(int r, int g, int b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public void SetColor(Color color)
    {
        r = (int)color.r;
        g = (int)color.g;
        b = (int)color.b;
    }

    // TODO : 1 do get color
    public Color GetColor()
    {
        if (r == 0 && g == 0 && b == 0) return ColorConstants.BLACK;
        else if (r == 1 && g == 1 && b == 1) return ColorConstants.WHITE;

        return new Color(r, g, b);
    }

    public bool IsEmpty()
    {
        return (r== 0 && g == 0 && b== 0);
    }

    public void RemoveColor()
    {
        r = 0; g=0; b=0;
    }

    public void BlendColor(ColorSet cSet)
    {
        r = Mathf.Max(r, cSet.r);
        g = Mathf.Max(g, cSet.g);
        b = Mathf.Max(b, cSet.b);
    }

    public override string ToString()
    {
        return "ColorSet : " + r + ", " + g + ", " + b; 
    }
}

public class GridInfo {

    private Vector2Int pos;
    private int height;
    private GridState state;
    private ColorSet colorSet;

    public Vector2Int Pos { get { return pos; } }
    public int Height { get { return height; } }
    public GridState State { get { return state; } }
    public ColorSet Colorset {  get { return colorSet; } }  

    public GridInfo(Vector2Int pos, int height, GridState state = GridState.NONE)
    {
        this.pos = pos;
        this.height = height;
        this.state = state;

        this.colorSet = new ColorSet(0, 0, 0);
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
                gridinfo.Colorset.SetColor(ColorConstants.RED);
                break;

            case GridState.END:
                GetComponent<Renderer>().material.color = Color.blue;
                gridinfo.Colorset.SetColor(ColorConstants.BLUE);
                break;

            case GridState.CAMERA:
                Camera.main.GetComponent<CameraController>().SetQuaterView(transform.position - new Vector3(0, transform.position.y, 0));
                break;
        }
    }

    public void AppearGrid(float duration = 1f)
    {
        transform.DOMoveY(gridinfo.Height - transform.localScale.y/2 - Constant.BOX_SIZE/2, duration).SetEase(Ease.InOutElastic);
    }
}
