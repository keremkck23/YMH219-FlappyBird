using System.Drawing.Drawing2D;
using FlappyBird.Core;
using FlappyBird.Models;

namespace FlappyBird.Rendering;

/// <summary>
/// Oyun nesnelerini ve arayuz katmanlarini GDI+ (Graphics) ile cizer.
/// </summary>
public class GameRenderer
{
    public void Render(Graphics g, GameEngine engine)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        DrawSky(g);
        DrawClouds(g, engine.Clouds);
        DrawGround(g, engine.Ground);
    }

    private static void DrawSky(Graphics g)
    {
        var skyRect = new Rectangle(0, 0, GameConfig.CanvasWidth, GameConfig.PlayableHeight);
        using var skyBrush = new LinearGradientBrush(
            skyRect,
            Color.FromArgb(92, 196, 245),
            Color.FromArgb(196, 238, 255),
            LinearGradientMode.Vertical);
        g.FillRectangle(skyBrush, skyRect);

        // Uzak sehir silueti
        using var silhouetteBrush = new SolidBrush(Color.FromArgb(95, 134, 198, 224));
        int baseLine = GameConfig.PlayableHeight;
        int[] heights = [48, 72, 36, 86, 58, 42, 78, 64, 52, 82, 44, 68];
        int buildingWidth = GameConfig.CanvasWidth / heights.Length;
        for (int i = 0; i < heights.Length; i++)
        {
            g.FillRectangle(
                silhouetteBrush,
                i * buildingWidth,
                baseLine - heights[i],
                buildingWidth - 2,
                heights[i]);
        }
    }

    private static void DrawClouds(Graphics g, IReadOnlyList<Cloud> clouds)
    {
        using var cloudBrush = new SolidBrush(Color.FromArgb(225, 255, 255, 255));
        foreach (Cloud cloud in clouds)
        {
            float s = cloud.Scale;
            g.FillEllipse(cloudBrush, cloud.X, cloud.Y + 10 * s, 46 * s, 26 * s);
            g.FillEllipse(cloudBrush, cloud.X + 18 * s, cloud.Y, 44 * s, 32 * s);
            g.FillEllipse(cloudBrush, cloud.X + 42 * s, cloud.Y + 8 * s, 42 * s, 26 * s);
        }
    }

    private static void DrawGround(Graphics g, Ground ground)
    {
        int topY = ground.TopY;
        int height = ground.Height;

        // Toprak tabakasi
        using var dirtBrush = new SolidBrush(Color.FromArgb(222, 214, 148));
        g.FillRectangle(dirtBrush, 0, topY, GameConfig.CanvasWidth, height);

        // Ust cim seridi
        const int grassHeight = 18;
        using var grassBrush = new SolidBrush(Color.FromArgb(115, 191, 46));
        g.FillRectangle(grassBrush, 0, topY, GameConfig.CanvasWidth, grassHeight);

        // Kayan capraz serit deseni
        using var stripeBrush = new SolidBrush(Color.FromArgb(88, 162, 34));
        for (float x = ground.OffsetX - 28; x < GameConfig.CanvasWidth + 28; x += 28)
        {
            PointF[] stripe =
            [
                new(x, topY + grassHeight),
                new(x + 12, topY + grassHeight),
                new(x + 20, topY + 2),
                new(x + 8, topY + 2)
            ];
            g.FillPolygon(stripeBrush, stripe);
        }

        using var borderPen = new Pen(Color.FromArgb(72, 118, 32), 2f);
        g.DrawLine(borderPen, 0, topY, GameConfig.CanvasWidth, topY);
        g.DrawLine(borderPen, 0, topY + grassHeight, GameConfig.CanvasWidth, topY + grassHeight);
    }
}
