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

    public bool IsOffScreen => X + Width < -10;

    public void Move(float speed)
    {
        X -= speed;
    }

    public void Reposition(float newX, int newGapY, int newGapHeight)
    {
        X = newX;
        GapY = newGapY;
        GapHeight = newGapHeight;
        IsPassed = false;
    }

    /// <summary>
    /// Kusun borunun orta noktasini ilk kez gecip gecmedigini kontrol eder.
    /// </summary>
    public bool TryMarkPassed(float birdX)
    {
        if (!IsPassed && X + Width < birdX)
        {
            IsPassed = true;
            return true;
        }

        return false;
    }
}
