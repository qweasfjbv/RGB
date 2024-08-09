using Fusion;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;

public class MaterialSetter : MonoBehaviour
{
    public BoxColorController boxColorController;
    void Start()
    {
        pbMesh = GetComponent<ProBuilderMesh>();

        faces = pbMesh.faces.ToArray();

        ColorPBMesh(0, ColorConstants.PURPLE);
        ColorPBMesh(1, ColorConstants.GREEN);
        ColorPBMesh(4, ColorConstants.ORANGE);

        
    }


    private BoxController boxController;
    private ProBuilderMesh pbMesh;

    private Face[] faces;





    private void ColorPBMesh(int idx, Color color)
    {
        ColorSet cSet = new ColorSet(color);
            pbMesh.SetFaceColor(faces[idx], color);
            pbMesh.ToMesh();
            pbMesh.Refresh();
    }




}
