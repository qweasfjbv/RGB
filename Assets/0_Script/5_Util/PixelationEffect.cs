using UnityEngine;

[ExecuteInEditMode]
public class PixelationEffect : MonoBehaviour
{
    public Material pixelationMaterial;
    [Range(1, 1024)]
    public int pixelDensity = 256;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (pixelationMaterial != null)
        {
            pixelationMaterial.SetFloat("_PixelDensity", pixelDensity);
            Graphics.Blit(source, destination, pixelationMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}