using UnityEngine;
using DG.Tweening;
using System;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.MeshOperations;
using System.Collections;

// Convert RGB to RYB
[Serializable]
public struct ColorSet : INetworkStruct
{
    public int r;
    public int y;
    public int b;

    public ColorSet(Color color)
    {
        r = 0; y = 0; b = 0;

        switch (color)
        {
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


    public void SetColor(Color color)
    {
        switch (color)
        {
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

    public int GetColorIdx()
    {
        Color color = GetColor();
        switch (color)
        {
            case var _ when color.Equals(ColorConstants.WHITE):
                return 0;
            case var _ when color.Equals(ColorConstants.RED):
                return 1;
            case var _ when color.Equals(ColorConstants.BLUE):
                return 2;
            case var _ when color.Equals(ColorConstants.YELLOW):
                return 3;
            case var _ when color.Equals(ColorConstants.GREEN):
                return 4;
            case var _ when color.Equals(ColorConstants.ORANGE):
                return 5;
            case var _ when color.Equals(ColorConstants.PURPLE):
                return 6;
            case var _ when color.Equals(ColorConstants.BLACK):
                return 7;
            default:
                return -1;

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
        return (r == 0 && y == 0 && b == 0);
    }

    public void RemoveColor()
    {
        r = 0; y = 0; b = 0;
    }

    public void GetBlendedColor(ColorSet cSet)
    {

        r = (r + cSet.r) > 1 ? 1 : r + cSet.r;
        y = (y + cSet.y) > 1 ? 1 : y + cSet.y;
        b = (b + cSet.b) > 1 ? 1 : b + cSet.b;
    }

    public void BlendColor(ColorSet cSet)
    {
        if (this.Equals(cSet))
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
        return HashCode.Combine(r, y, b);
    }
}

[Serializable]
public struct NetworkGridInfo : INetworkStruct{

    [Networked] public Vector2Int Pos { get; set; }
    [Networked] public int Height { get; set; }
    [Networked] public GridState State { get; set; }
    [Networked] public ColorSet colorset { get; set; }


    public NetworkGridInfo(Vector2Int pos, int height, int colorIdx, GridState state)
    {
        this.Pos = pos;
        this.Height = height;

        if (state < 0) this.State = 0;
        else this.State = state;


        colorset = new ColorSet(ColorConstants.COLORARR[colorIdx]);
    }

}


public class MapGrid : NetworkBehaviour
{
    [SerializeField, Networked]
    public NetworkGridInfo NetworkedGridInfo { get; set; }

    // Process Grid According to GridState
    public void InitMapGrid(NetworkGridInfo info)
    {
        NetworkedGridInfo = new NetworkGridInfo(info.Pos, info.Height, info.colorset.GetColorIdx(), info.State);

        RPC_UpdateGridVisuals();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_UpdateGridVisuals()
    {
        transform.localScale = Vector3.one * Constant.GRID_SIZE;
        transform.position = new Vector3(transform.position.x, NetworkedGridInfo.Height * Constant.GRID_SIZE - transform.localScale.y / 2 - Constant.BOX_SIZE / 2, transform.position.z);

        if (GameManagerEx.Instance.IsColorBlind)
        {
            GetComponent<Renderer>().material.mainTexture = Managers.Resource.GetDiceTexture(NetworkedGridInfo.colorset.GetColorIdx());
        }
        else
        {
            GetComponent<Renderer>().material.color = NetworkedGridInfo.colorset.GetColor();
        }
        switch (NetworkedGridInfo.State)
        {
            case GridState.CAMERA:
                Camera.main.GetComponent<CameraController>().SetQuaterView(transform.position - new Vector3(0, transform.position.y, 0));
                break;
        }
    }

    public void SetGridInfo(NetworkGridInfo info)
    {
        NetworkedGridInfo = info;

        RPC_UpdateGridVisuals();
    }
}
