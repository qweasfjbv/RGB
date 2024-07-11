using UnityEngine;

[ExecuteInEditMode]
public class ColorCorrectionEffect : MonoBehaviour
{
    public Shader colorCorrectionShader;
    private Material colorCorrectionMaterial;

    void Start()
    {
        if (colorCorrectionShader == null)
        {
            Debug.LogError("No shader assigned!");
            enabled = false;
            return;
        }

        colorCorrectionMaterial = new Material(colorCorrectionShader);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (colorCorrectionMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        Graphics.Blit(source, destination, colorCorrectionMaterial);
    }
}