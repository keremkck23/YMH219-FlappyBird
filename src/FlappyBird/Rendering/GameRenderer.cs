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
        DrawFloatingTexts(g, engine.FloatingTexts);
        DrawLiveScore(g, engine);
        DrawStatusOverlay(g, engine);
    }

    private static void DrawFloatingTexts(Graphics g, IReadOnlyList<FloatingText> texts)
    {
        using var font = new Font("Segoe UI", 14f, FontStyle.Bold);
        foreach (FloatingText item in texts)
        {
            int alpha = (int)(item.Opacity * 255);
            using var shadowBrush = new SolidBrush(Color.FromArgb(alpha / 2, 0, 0, 0));
            using var textBrush = new SolidBrush(Color.FromArgb(alpha, 255, 252, 160));
            g.DrawString(item.Text, font, shadowBrush, item.X + 1.5f, item.Y + 1.5f);
            g.DrawString(item.Text, font, textBrush, item.X, item.Y);
        }
    }

    private static void DrawLiveScore(Graphics g, GameEngine engine)
    {
        if (engine.Status == GameStatus.Ready)
        {
            return;
        }

        string scoreText = engine.Score.ToString();
        using var font = new Font("Segoe UI", 34f, FontStyle.Bold);
        SizeF size = g.MeasureString(scoreText, font);
        float x = (GameConfig.CanvasWidth - size.Width) / 2f;
        const float y = 24f;

        using var shadowBrush = new SolidBrush(Color.FromArgb(145, 0, 0, 0));
        g.DrawString(scoreText, font, shadowBrush, x + 3, y + 3);
        g.DrawString(scoreText, font, Brushes.White, x, y);
    }

    private static void DrawStatusOverlay(Graphics g, GameEngine engine)
    {
        switch (engine.Status)
        {
            case GameStatus.Ready:
                DrawCenterCard(
                    g,
                    title: "FLAPPY BIRD",
                    titleColor: Color.FromArgb(255, 212, 48),
                    lines:
                    [
                        "Ziplamak icin SPACE / YUKARI OK / W",
                        "Duraklatmak icin P  |  Ses icin M",
                        $"En Yuksek Skor: {engine.HighScore}"
                    ],
                    footer: "Baslamak icin SPACE tusuna veya ekrana tiklayin");
                break;

            case GameStatus.Paused:
                DrawCenterCard(
                    g,
                    title: "DURAKLATILDI",
                    titleColor: Color.FromArgb(120, 210, 255),
                    lines:
                    [
                        $"Mevcut Skor: {engine.Score}",
                        $"Hiz Seviyesi: {engine.CurrentSpeed:F1}x"
                    ],
                    footer: "Devam etmek icin P tusuna basin");
                break;

            case GameStatus.GameOver:
                string badge = engine.Score >= 15 ? "Altin Madalya" :
                               engine.Score >= 8 ? "Gumus Madalya" :
                               engine.Score >= 3 ? "Bronz Madalya" : "Yeni Deneme";
                DrawCenterCard(
                    g,
                    title: "GAME OVER",
                    titleColor: Color.FromArgb(255, 102, 82),
                    lines:
                    [
                        $"Skorunuz: {engine.Score}",
                        $"En Yuksek Skor: {engine.HighScore}",
                        $"Basari: {badge}"
                    ],
                    footer: "Yeniden baslamak icin R / ENTER tusuna basin");
                break;
        }
    }

    private static void DrawCenterCard(
        Graphics g,
        string title,
        Color titleColor,
        string[] lines,
        string footer)
    {
        const int cardWidth = 370;
        const int cardHeight = 220;
        int x = (GameConfig.CanvasWidth - cardWidth) / 2;
        int y = (GameConfig.PlayableHeight - cardHeight) / 2 - 12;

        using var overlayBrush = new SolidBrush(Color.FromArgb(75, 12, 24, 38));
        g.FillRectangle(overlayBrush, 0, 0, GameConfig.CanvasWidth, GameConfig.PlayableHeight);

        using var cardPath = CreateRoundedRectangle(new Rectangle(x, y, cardWidth, cardHeight), 16);
        using var cardBrush = new SolidBrush(Color.FromArgb(228, 24, 36, 52));
        using var borderPen = new Pen(Color.FromArgb(220, 255, 255, 255), 2f);
        g.FillPath(cardBrush, cardPath);
        g.DrawPath(borderPen, cardPath);

        using var titleFont = new Font("Segoe UI", 24f, FontStyle.Bold);
        using var bodyFont = new Font("Segoe UI", 11.5f, FontStyle.Regular);
        using var footerFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        using var titleBrush = new SolidBrush(titleColor);
        using var format = new StringFormat { Alignment = StringAlignment.Center };

        g.DrawString(title, titleFont, titleBrush, new RectangleF(x, y + 18, cardWidth, 44), format);

        float lineY = y + 76;
        foreach (string line in lines)
        {
            g.DrawString(line, bodyFont, Brushes.WhiteSmoke, new RectangleF(x + 12, lineY, cardWidth - 24, 24), format);
            lineY += 26;
        }

        using var footerBrush = new SolidBrush(Color.FromArgb(255, 232, 130));
        g.DrawString(footer, footerFont, footerBrush, new RectangleF(x + 10, y + cardHeight - 36, cardWidth - 20, 24), format);
    }

    private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int diameter = radius * 2;
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
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
