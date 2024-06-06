using System.Linq;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

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

    private void Start()
    {
        ProBuilderMesh pbMesh = GetComponent<ProBuilderMesh>();

        if (pbMesh == null)
        {
            Debug.Log("No Probuilder Mesh on Obj"); return;
        }

        Face[] faces = pbMesh.faces.ToArray();

        for (int i = 0; i < faces.Length; i++)
        {
            Color faceColor = colors[i % colors.Length];

            if (i >= colors.Length)
            {
                faceColor = Color.white;
            }
            pbMesh.SetFaceColor(faces[i], faceColor);
        }

        pbMesh.ToMesh();
        pbMesh.Refresh();
        pbMesh.Optimize();
    }
}
