using FlappyBird.Core;

namespace FlappyBird.Models;

/// <summary>
/// Oyuncunun kontrol ettigi kus nesnesini temsil eder.
/// </summary>
public class Bird
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public int Width { get; }
    public int Height { get; }
    public float Velocity { get; private set; }

    /// <summary>
    /// Carpisma kontrolu icin hafif daraltilmis sinir dikdortgeni.
    /// </summary>
    public RectangleF Bounds => new(X + 4, Y + 4, Width - 8, Height - 8);

    public Bird()
    {
        Width = GameConfig.BirdWidth;
        Height = GameConfig.BirdHeight;
        X = GameConfig.BirdStartX;
        Y = GameConfig.BirdStartY;
        Velocity = 0f;
    }
}
