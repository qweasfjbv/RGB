using UnityEngine;

public static class Constant
{
    public const float GRID_SIZE    = 0.6f;
    public const float BOX_SIZE     = 0.5f;


}

public static class ColorConstants {

    // TODO : Color Modify Needed
    public static readonly Color RED = new Color(255f/255, 50f/255, 50f/255, 1f);
    public static readonly Color BLUE = new Color(50f / 255, 50f / 255, 255f / 255, 1f);
    public static readonly Color YELLOW = new Color(255f / 255, 255f / 255, 30f / 255, 1f);
    public static readonly Color GREEN = new Color(50f / 255, 255f / 255, 255f / 255, 1f);
    public static readonly Color ORANGE = new Color(255f / 255, 127f / 255, 0f / 255, 1f);
    public static readonly Color PURPLE = new Color(113f / 255, 0f / 147, 255f / 255, 1f);
    public static readonly Color BLACK = new Color(20f/255, 20f/255, 20f/255, 1f);
    public static readonly Color WHITE = new Color(1f, 1f, 1f, 1f);

    public static Color[] COLORARR = new Color[]{ RED, BLUE, YELLOW, GREEN, ORANGE, PURPLE, BLACK, WHITE };
}
