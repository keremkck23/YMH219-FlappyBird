namespace FlappyBird.Models;

/// <summary>
/// Skor artisi sirasinda yukari dogru suzulup kaybolan bildirim metni (+1).
/// </summary>
public class FloatingText
{
    public string Text { get; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public int RemainingFrames { get; private set; }
    public int TotalFrames { get; }

    public bool IsExpired => RemainingFrames <= 0;
    public float Opacity => Math.Clamp((float)RemainingFrames / TotalFrames, 0f, 1f);

    public FloatingText(string text, float x, float y, int durationFrames = 32)
    {
        Text = text;
        X = x;
        Y = y;
        TotalFrames = durationFrames;
        RemainingFrames = durationFrames;
    }

    public void Update()
    {
        Y -= 1.4f;
        RemainingFrames--;
    }
}
