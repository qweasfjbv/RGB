using UnityEngine;

[ExecuteInEditMode]
public class ColorBlindnessEffect : MonoBehaviour
{
    public Shader colorBlindnessShader;
    private Material colorBlindnessMaterial;

    public enum ColorBlindnessType
    {
        Normal,
        Protanopia,
        Deuteranopia
    }

    public ColorBlindnessType blindnessType = ColorBlindnessType.Normal;

    void Start()
    {
        if (colorBlindnessShader == null)
        {
            Debug.LogError("No shader assigned!");
            enabled = false;
            return;
        }

        colorBlindnessMaterial = new Material(colorBlindnessShader);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (colorBlindnessMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        colorBlindnessMaterial.SetFloat("_Type", (float)blindnessType);
        Graphics.Blit(source, destination, colorBlindnessMaterial);
    }
}