using FlappyBird.Core;

namespace FlappyBird.Models;

/// <summary>
/// Gokyuzunde paralaks efektiyle yavasca hareket eden bulutlari temsil eder.
/// </summary>
public class Cloud
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Scale { get; private set; }
    public float SpeedFactor { get; private set; }

    public Cloud(float x, float y, float scale, float speedFactor)
    {
        X = x;
        Y = y;
        Scale = scale;
        SpeedFactor = speedFactor;
    }

    public void Update(float baseSpeed)
    {
        X -= baseSpeed * SpeedFactor;
        if (X < -100 * Scale)
        {
            X = GameConfig.CanvasWidth + 30;
        }
    }
}
