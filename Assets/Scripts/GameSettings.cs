using UnityEngine;

public static class GameSettings
{
    public const int CellCountX = 7;
    public const int CellCountY = 6;

    public const float CellSizeX = 162f;
    public const float CellSizeY = 186f;
    
    public const int CellTypeCount = 4;
    public const float durationAnimation = .8f;

    public const int turnMax = 20;
    public const int scoreCell = 100;
    public const int scoreBonusMultiplier = 2;

    public readonly static Vector2 originOffset;

    static GameSettings()
    {
        originOffset = new Vector2(
            (-1) * (CellCountX - 1) * CellSizeX * .5f,
            ((-1) * (CellCountY - 1) * CellSizeY * .5f) - (CellSizeY * .25f));
    }
}
