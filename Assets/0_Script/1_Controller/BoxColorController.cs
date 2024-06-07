using System.Linq;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;

public class BoxColorController : MonoBehaviour
{
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
        for (int i = 0; i < 6; i++) boxColorSet[i] = new ColorSet(0, 0, 0);

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
