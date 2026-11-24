using FlappyBird.Core;

namespace FlappyBird.Models;

/// <summary>
/// Ekranda sagdan sola hareket eden ust ve alt boru engel ciftini temsil eder.
/// </summary>
public class PipeObstacle
{
    public float X { get; private set; }
    public int Width { get; }
    public int GapY { get; private set; }
    public int GapHeight { get; private set; }
    public bool IsPassed { get; private set; }

    public RectangleF TopBounds => new(X, 0, Width, GapY);

    public RectangleF BottomBounds
    {
        get
        {
            float bottomY = GapY + GapHeight;
            return new RectangleF(X, bottomY, Width, GameConfig.PlayableHeight - bottomY);
        }
    }

    public PipeObstacle(float startX, int gapY, int gapHeight)
    {
        X = startX;
        Width = GameConfig.PipeWidth;
        GapY = gapY;
        GapHeight = gapHeight;
        IsPassed = false;
    }
}
