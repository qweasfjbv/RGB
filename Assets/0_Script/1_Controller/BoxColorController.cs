using Fusion;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class BoxColorController : NetworkBehaviour
{
    private BoxController boxController;
    private ProBuilderMesh pbMesh;

    private Face[] faces;

    [Networked, Capacity(6), UnitySerializeField]
    private NetworkArray<ColorSet> NetworkBoxColorset { get; } =
        MakeInitializer(new ColorSet[]{
        new ColorSet(ColorConstants.WHITE),
        new ColorSet(ColorConstants.WHITE),
        new ColorSet(ColorConstants.WHITE),
        new ColorSet(ColorConstants.WHITE),
        new ColorSet(ColorConstants.WHITE),
        new ColorSet(ColorConstants.WHITE)});

    private void Start()
    {
        pbMesh = GetComponent<ProBuilderMesh>();
        boxController = GetComponent<BoxController>();

        if (pbMesh == null)
        {
            Debug.Log("No Probuilder Mesh on Obj"); return;
        }

        faces = pbMesh.faces.ToArray();

    }

    // Order : Top, Back, Right, Forward, Left, Bottom 
    private int DirToFaceIdx(BoxDir dir) => dir switch
    {
        BoxDir.TOP => 0,
        BoxDir.BACK => 1,
        BoxDir.RIGHT => 2,
        BoxDir.FORWARD => 3,
        BoxDir.LEFT => 4,
        BoxDir.BOTTOM => 5,
        _ => -1
    };

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_ColorPBMesh(int idx, Color color)
    {
        Debug.Log("COLOR CHAGNE");
        pbMesh.SetFaceColor(faces[idx], color);
        pbMesh.ToMesh();
        pbMesh.Refresh();
    }

    public void SetBoxColor(BoxDir dir, Color color)
    {
        int idx = DirToFaceIdx(dir);

        var cSet = NetworkBoxColorset[(int)dir];
        cSet.SetColor(color);
        NetworkBoxColorset.Set((int)dir, cSet);

        RPC_ColorPBMesh(idx, color);
    }


    public void RemoveBoxColor(BoxDir dir)
    {
        int idx = DirToFaceIdx(dir);

        var cSet = NetworkBoxColorset[(int)dir];
        cSet.SetColor(Color.white);
        NetworkBoxColorset.Set((int)dir, cSet);


        RPC_ColorPBMesh(idx, ColorConstants.WHITE);

    }

    public void StampColor(BoxDir dir)
    {
        if (!Runner.IsSharedModeMasterClient) return;

        ColorSet gridC = MapGenerator.Instance.GetGridColor(boxController.GetBoxPos());
        ColorSet boxC = NetworkBoxColorset[(int)dir];

        // Empty-> get Color from grid
        if (NetworkBoxColorset[(int)dir].IsEmpty())
        {
            boxC.BlendColor(gridC);
            gridC.RemoveColor();

        }
        // !Empty -> Blend color to grid
        else
        {
            gridC.BlendColor(boxC);
            boxC.RemoveColor();
        }


        SetBoxColor(dir, boxC.GetColor());
        MapGenerator.Instance.SetGridColor(boxController.GetBoxPos(), gridC.GetColor());

    }

    public Color GetBlendColorWithFloor()
    {
        
        ColorSet cs = new ColorSet(NetworkBoxColorset[(int)boxController.BoxDirs[(int)BoxDir.BOTTOM]].GetColor());


        cs.GetBlendedColor(MapGenerator.Instance.GetGridColor(boxController.GetBoxPos()));

        return cs.GetColor();
    }

    public bool CheckBoxClear()
    {
        for (int i = 0; i < 6; i++)
        {
            if (!NetworkBoxColorset[i].IsEmpty())
            {
                return false;
            }
        }

        return true;
    }

    /*
    public void AddBoxColor(BoxDir dir, Color color)
    {
        int idx = DirToFaceIdx(dir);


        pbMesh.SetFaceColor(faces[idx], ColorConstants.WHITE);
        pbMesh.ToMesh();
        pbMesh.Refresh();
        pbMesh.Optimize();
    }
    */
}
