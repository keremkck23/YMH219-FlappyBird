namespace FlappyBird.Controls;

/// <summary>
/// Titremesiz (flicker-free) 60 FPS cizim icin cift tamponlu ozel Panel kontrolu.
/// </summary>
public sealed class GameCanvasPanel : Panel
{
    public GameCanvasPanel()
    {
        DoubleBuffered = true;
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);
        UpdateStyles();
    }
}
