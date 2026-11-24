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
    public float RotationAngle { get; private set; }
    public float WingPhase { get; private set; }

    /// <summary>
    /// Kanat animasyonu icin dikey salinim ofseti.
    /// </summary>
    public float WingOffset => (float)Math.Sin(WingPhase) * 4.5f;

    /// <summary>
    /// Carpisma kontrolu icin hafif daraltilmis sinir dikdortgeni.
    /// </summary>
    public RectangleF Bounds => new(X + 4, Y + 4, Width - 8, Height - 8);

    public Bird()
    {
        Width = GameConfig.BirdWidth;
        Height = GameConfig.BirdHeight;
        Reset();
    }

    public void Reset()
    {
        X = GameConfig.BirdStartX;
        Y = GameConfig.BirdStartY;
        Velocity = 0f;
        RotationAngle = 0f;
        WingPhase = 0f;
    }

    public void Jump()
    {
        Velocity = GameConfig.JumpVelocity;
        RotationAngle = -26f;
        WingPhase += 1.4f;
    }

    public void ApplyGravity()
    {
        Velocity = Math.Min(Velocity + GameConfig.Gravity, GameConfig.MaxFallSpeed);
        Y += Velocity;
        WingPhase += Velocity < 0 ? 0.45f : 0.22f;

        // Hiza gore dogal burun egimi
        float targetAngle = Math.Clamp(Velocity * 5.2f, -26f, 70f);
        RotationAngle += (targetAngle - RotationAngle) * 0.25f;
    }

    /// <summary>
    /// Baslangic ekraninda kusun havada hafifce salinmasini saglar.
    /// </summary>
    public void UpdateIdleHover()
    {
        WingPhase += 0.18f;
        Y = GameConfig.BirdStartY + (float)Math.Sin(WingPhase * 0.7f) * 7f;
        RotationAngle = (float)Math.Sin(WingPhase * 0.7f) * 5f;
    }

    public void ClampToGround()
    {
        float maxY = GameConfig.PlayableHeight - Height;
        if (Y > maxY)
        {
            Y = maxY;
            Velocity = 0f;
        }
    }
}
