namespace FlappyBird.Core;

/// <summary>
/// Oyunun temel boyut, fizik ve zorluk sabitlerini tutar.
/// </summary>
public static class GameConfig
{
    // Pencere ve oyun alani boyutlari
    public const int CanvasWidth = 480;
    public const int CanvasHeight = 620;
    public const int TopBarHeight = 48;
    public const int GroundHeight = 84;
    public const int PlayableHeight = CanvasHeight - GroundHeight;

    // Zamanlayici (yaklasik 60 FPS)
    public const int TimerIntervalMs = 16;

    // Kus fizigi
    public const int BirdStartX = 115;
    public const int BirdStartY = 230;
    public const int BirdWidth = 40;
    public const int BirdHeight = 30;
    public const float Gravity = 0.52f;
    public const float JumpVelocity = -8.6f;
    public const float MaxFallSpeed = 11.5f;

    // Boru engelleri
    public const int PipeCount = 3;
    public const int PipeWidth = 68;
    public const int PipeHorizontalSpacing = 225;
    public const int InitialPipeGap = 158;
    public const int MinPipeGap = 118;
    public const int MinPipeTopHeight = 65;
    public const float InitialPipeSpeed = 3.2f;
    public const float MaxPipeSpeed = 6.2f;
    public const float SpeedIncreasePerPoint = 0.12f;
    public const int GapDecreasePerFivePoints = 6;
}
