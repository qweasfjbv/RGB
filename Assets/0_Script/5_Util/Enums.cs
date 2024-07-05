using System;


[Serializable]
public enum GridState { NONE = 0, START, CAMERA }

public enum BoxDir { FORWARD = 0, BOTTOM, BACK, TOP, LEFT, RIGHT }

public enum EffectClip { BASIC_JUMP =0, STAMP, BOX_TURN, SHADE, FALL}

public enum BGMClip { MAIN_BGM = 0, GAME_BGM}