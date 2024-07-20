using UnityEngine;

public static class Constant
{
    public const float GRID_SIZE    = 0.6f;
    public const float BOX_SIZE     = 0.5f;


    public const string GAME_SCENE = "GameScene";
    public const string MAIN_SCENE = "MainScene";

    public const string HINT_TABLE = "Hints";

}

public static class ColorConstants {

    // Primary Colors
    public static readonly Color RED = new Color(255f / 255, 50f / 255, 50f / 255, 1f);
    public static readonly Color BLUE = new Color(50f / 255, 50f / 255, 255f / 255, 1f);
    public static readonly Color YELLOW = new Color(255f / 255, 255f / 255, 30f / 255, 1f);
    public static readonly Color GREEN = new Color(50f / 255, 255f / 255, 50f / 255, 1f);
    public static readonly Color ORANGE = new Color(255f / 255, 127f / 255, 0f / 255, 1f);
    public static readonly Color PURPLE = new Color(149f / 255, 91f / 255, 236f / 255, 1f);
    public static readonly Color BLACK = new Color(20f / 255, 20f / 255, 20f / 255, 1f);
    public static readonly Color WHITE = new Color(1f, 1f, 1f, 1f);

    public static Color[] COLORARR = new Color[]{ WHITE, RED, BLUE, YELLOW, GREEN, ORANGE, PURPLE, BLACK };

    // Pastel Tone colors
    public static readonly Color P_RED = new Color(255f / 255, 150f / 255, 150f / 255, 1f);
    public static readonly Color P_BLUE = new Color(150f / 255, 150f / 255, 255f / 255, 1f);
    public static readonly Color P_YELLOW = new Color(255f / 255, 255f / 255, 100f / 255, 1f);
    public static readonly Color P_GREEN = new Color(184f / 255, 235f / 255, 130f / 255, 1f);
    public static readonly Color P_ORANGE = new Color(255f / 255, 175f / 255, 86f / 255, 1f);
    public static readonly Color P_PURPLE = new Color(191f / 255, 159f / 255, 238f / 255, 1f);
    public static readonly Color P_BLACK = new Color(20f / 255, 20f / 255, 20f / 255, 1f);
    public static readonly Color P_WHITE = new Color(1f, 1f, 1f, 1f);

    public static Color[] PCOLORARR = new Color[] { P_WHITE, P_RED, P_BLUE, P_YELLOW, P_GREEN, P_ORANGE, P_PURPLE, P_BLACK };

}
