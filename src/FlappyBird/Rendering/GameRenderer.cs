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
        DrawPipes(g, engine.Pipes);
        DrawGround(g, engine.Ground);
        DrawBird(g, engine.Bird);
    }

    private static void DrawBird(Graphics g, Bird bird)
    {
        GraphicsState state = g.Save();

        float centerX = bird.X + bird.Width / 2f;
        float centerY = bird.Y + bird.Height / 2f;

        g.TranslateTransform(centerX, centerY);
        g.RotateTransform(bird.RotationAngle);

        float halfW = bird.Width / 2f;
        float halfH = bird.Height / 2f;

        // Kus govdesi
        var bodyRect = new RectangleF(-halfW, -halfH, bird.Width, bird.Height);
        using var bodyBrush = new LinearGradientBrush(
            bodyRect,
            Color.FromArgb(255, 232, 72),
            Color.FromArgb(245, 168, 28),
            LinearGradientMode.Vertical);
        using var outlinePen = new Pen(Color.FromArgb(72, 46, 18), 2f);

        g.FillEllipse(bodyBrush, bodyRect);
        g.DrawEllipse(outlinePen, bodyRect);

        // Acik renkli karin bolgesi
        using var bellyBrush = new SolidBrush(Color.FromArgb(255, 246, 178));
        g.FillEllipse(bellyBrush, -halfW + 6, 1, bird.Width - 14, halfH - 2);

        // Hareketli kanat
        var wingRect = new RectangleF(-halfW + 5, -2 + bird.WingOffset * 0.55f, 18, 12);
        using var wingBrush = new SolidBrush(Color.FromArgb(255, 250, 215));
        g.FillEllipse(wingBrush, wingRect);
        g.DrawEllipse(outlinePen, wingRect);

        // Goz
        var eyeRect = new RectangleF(4, -halfH + 4, 11, 11);
        g.FillEllipse(Brushes.White, eyeRect);
        g.DrawEllipse(outlinePen, eyeRect);
        g.FillEllipse(Brushes.Black, 10, -halfH + 7, 4.5f, 4.5f);
        g.FillEllipse(Brushes.White, 11.5f, -halfH + 8, 1.8f, 1.8f);

        // Gaga
        PointF[] beak =
        [
            new(halfW - 3, -2),
            new(halfW + 10, 2),
            new(halfW - 3, 7)
        ];
        using var beakBrush = new SolidBrush(Color.FromArgb(242, 92, 42));
        g.FillPolygon(beakBrush, beak);
        g.DrawPolygon(outlinePen, beak);

        g.Restore(state);
    }

    private static void DrawPipes(Graphics g, IReadOnlyList<PipeObstacle> pipes)
    {
        foreach (PipeObstacle pipe in pipes)
        {
            DrawPipeSection(g, pipe.TopBounds, isTopPipe: true);
            DrawPipeSection(g, pipe.BottomBounds, isTopPipe: false);
        }
    }

    private static void DrawPipeSection(Graphics g, RectangleF rect, bool isTopPipe)
    {
        if (rect.Height <= 0)
        {
            return;
        }

        using var bodyBrush = new LinearGradientBrush(
            rect,
            Color.FromArgb(148, 224, 68),
            Color.FromArgb(62, 142, 32),
            LinearGradientMode.Horizontal);

        var blend = new ColorBlend
        {
            Colors =
            [
                Color.FromArgb(95, 178, 44),
                Color.FromArgb(164, 236, 86),
                Color.FromArgb(88, 170, 38),
                Color.FromArgb(52, 122, 26)
            ],
            Positions = [0.0f, 0.24f, 0.68f, 1.0f]
        };
        bodyBrush.InterpolationColors = blend;

        using var outlinePen = new Pen(Color.FromArgb(44, 78, 22), 2.2f);
        g.FillRectangle(bodyBrush, rect);
        g.DrawRectangle(outlinePen, rect.X, rect.Y, rect.Width, rect.Height);

        // Boru kapagi (flans)
        const float capHeight = 24f;
        const float capOverhang = 5f;
        float capY = isTopPipe ? rect.Bottom - capHeight : rect.Top;
        var capRect = new RectangleF(rect.X - capOverhang, capY, rect.Width + capOverhang * 2, capHeight);

        g.FillRectangle(bodyBrush, capRect);
        g.DrawRectangle(outlinePen, capRect.X, capRect.Y, capRect.Width, capRect.Height);
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
