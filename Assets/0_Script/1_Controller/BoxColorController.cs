using System.Linq;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;

public class BoxColorController : MonoBehaviour
{
    private BoxController boxController;

    // Top, Back, Right, Forward, Left, Bottom
    private Color[] colors = new Color[]
    { 
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    private ProBuilderMesh pbMesh;
    private Face[] faces;
    private ColorSet[] boxColorSet = new ColorSet[6];

    private void Start()
    {
        pbMesh = GetComponent<ProBuilderMesh>();
        boxController = GetComponent<BoxController>();  
        for (int i = 0; i < 6; i++) boxColorSet[i] = new ColorSet(ColorConstants.WHITE);

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

    public void SetBoxColor(BoxDir dir, Color color)
    {
        int idx = DirToFaceIdx(dir);

        boxColorSet[(int)dir].SetColor(color);
        pbMesh.SetFaceColor(faces[idx], color);
        pbMesh.ToMesh();
        pbMesh.Refresh();
        pbMesh.Optimize();

    }

    public void RemoveBoxColor(BoxDir dir)
    {
        int idx = DirToFaceIdx(dir);

        boxColorSet[(int)dir].SetColor(Color.clear);

        pbMesh.SetFaceColor(faces[idx], ColorConstants.WHITE);
        pbMesh.ToMesh();
        pbMesh.Refresh();
        pbMesh.Optimize();
    }

    public void TestToggle(BoxDir dir)
    {
        if (boxColorSet[(int)dir].IsEmpty())
        {
            Debug.Log(boxColorSet[(int)dir].ToString());
            SetBoxColor(dir, ColorConstants.RED);
        }
        else RemoveBoxColor(dir);
    }

    public void StampColor(BoxDir dir)
    {
        ColorSet gridC = MapGenerator.Instance.GetGridColor(boxController.GetBoxPos());
        ColorSet boxC = boxColorSet[(int)dir];

        // Empty-> get Color from grid
        if (boxColorSet[(int)dir].IsEmpty())
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
        ColorSet cs = new ColorSet(boxColorSet[(int)boxController.BoxDirs[(int)BoxDir.BOTTOM]].GetColor());

        cs.BlendColor(MapGenerator.Instance.GetGridColor(boxController.GetBoxPos()));

        return cs.GetColor();
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
