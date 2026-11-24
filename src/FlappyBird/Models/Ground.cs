using FlappyBird.Core;

namespace FlappyBird.Models;

/// <summary>
/// Oyunun alt kisminda surekli sola kayan zemini temsil eder.
/// </summary>
public class Ground
{
    private const int StripeWidth = 28;

    public float OffsetX { get; private set; }
    public int TopY => GameConfig.PlayableHeight;
    public int Height => GameConfig.GroundHeight;

    public RectangleF Bounds => new(0, TopY, GameConfig.CanvasWidth, Height);

    public void Update(float speed)
    {
        OffsetX -= speed;
        if (OffsetX <= -StripeWidth)
        {
            OffsetX += StripeWidth;
        }
    }

    public void Reset()
    {
        OffsetX = 0f;
    }
}
