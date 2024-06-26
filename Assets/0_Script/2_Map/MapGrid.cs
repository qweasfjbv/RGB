using UnityEngine;
using DG.Tweening;
using System;
using JetBrains.Annotations;

// Convert RGB to RYB
[Serializable]
public class ColorSet
{
    public int r;
    public int y;
    public int b;

    public int Total { get => (r + y + b); }

    public ColorSet(Color color)
    {
        SetColor(color);
    }

    public void SetColor(Color color)
    {

        switch (color) {
            case var _ when color.Equals(ColorConstants.RED):
                r = 1; y = 0; b = 0;
                break;
            case var _ when color.Equals(ColorConstants.BLUE):
                r = 0; y = 0; b = 1;
                break;
            case var _ when color.Equals(ColorConstants.YELLOW):
                r = 0; y = 1; b = 0;
                break;
            case var _ when color.Equals(ColorConstants.ORANGE):
                r = 1; y = 1; b = 0;
                break;
            case var _ when color.Equals(ColorConstants.GREEN):
                r = 0; y = 1; b = 1;
                break;
            case var _ when color.Equals(ColorConstants.PURPLE):
                r = 1; y = 0; b = 1;
                break;
            case var _ when color.Equals(ColorConstants.BLACK):
                r = 1; y = 1; b = 1;
                break;
            case var _ when color.Equals(ColorConstants.WHITE):
                r = 0; y = 0; b = 0;
                break;
            default:
                Debug.LogError("Color Error : color can't maching ryb");
                break;

        }
    }

    public Color GetColor()
    {
        if (r == 0 && y == 0 && b == 0) return ColorConstants.WHITE;
        else if (r == 1 && y == 1 && b == 1) return ColorConstants.BLACK;
        else if (r == 1 && y == 0 && b == 0) return ColorConstants.RED;
        else if (r == 0 && y == 1 && b == 0) return ColorConstants.YELLOW;
        else if (r == 0 && y == 0 && b == 1) return ColorConstants.BLUE;
        else if (r == 0 && y == 1 && b == 1) return ColorConstants.GREEN;
        else if (r == 1 && y == 1 && b == 0) return ColorConstants.ORANGE;
        else if (r == 1 && y == 0 && b == 1) return ColorConstants.PURPLE;

        Debug.LogError("Color Error : ryb can't matching color"); return Color.clear;
    }

    public bool IsEmpty()
    {
        return (r== 0 && y == 0 && b== 0);
    }

    public void RemoveColor()
    {
        r = 0; y=0; b=0;
    }

    public void GetBlendedColor(ColorSet cSet)
    {

        r = (r + cSet.r) > 1 ? 1 : r + cSet.r;
        y = (y + cSet.y) > 1 ? 1 : y + cSet.y;
        b = (b + cSet.b) > 1 ? 1 : b + cSet.b;
    }

    public void BlendColor(ColorSet cSet)
    {
        if(this.Equals(cSet))
        {
            cSet.RemoveColor();
            this.RemoveColor();
            return;
        }

        r = (r + cSet.r) > 1 ? 1 : r + cSet.r;
        y = (y + cSet.y) > 1 ? 1 : y + cSet.y;
        b = (b + cSet.b) > 1 ? 1 : b + cSet.b;
    }

    public override string ToString()
    {
        return "ColorSet(RYB) : " + r + ", " + y + ", " + b; 
    }

    public override bool Equals(object obj)
    {
        return obj is ColorSet c && c.r == r && c.b == b && c.y == y;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
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

    public GridInfo(Vector2Int pos, int height, int colorIdx, GridState state)
    {
        this.pos = pos;
        this.height = height;

        if (state < 0) this.state = 0;
        else this.state = state;

        colorSet = new ColorSet(ColorConstants.COLORARR[colorIdx]);
    }
}

public class MapGrid : MonoBehaviour
{
    private GridInfo gridinfo;
    public GridInfo Gridinfo { get { return gridinfo; } }   


    // Process Grid According to GridState
    public void InitMapGrid(GridInfo info)
    {
        gridinfo = info;
        GetComponent<Renderer>().material.color = info.Colorset.GetColor();

        switch (info.State)
        {
            case GridState.NONE: break;
            case GridState.START:
                break;
            case GridState.CAMERA:
                Camera.main.GetComponent<CameraController>().SetQuaterView(transform.position - new Vector3(0, transform.position.y, 0));
                break;

        }
    }

    public void AppearGrid(float duration = 1f)
    {
        transform.DOMoveY(gridinfo.Height * Constant.GRID_SIZE - transform.localScale.y/2 - Constant.BOX_SIZE/2, duration).SetEase(Ease.InOutElastic);
    }

    public void DisappearGrid(float duration = 1f)
    {
        transform.DOMoveY(gridinfo.Height * Constant.GRID_SIZE - 10, duration).SetEase(Ease.InOutElastic).OnComplete(()=>Destroy(gameObject));
    }

}
