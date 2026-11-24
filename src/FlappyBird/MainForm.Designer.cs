using FlappyBird.Controls;
using FlappyBird.Core;

namespace FlappyBird;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private GameCanvasPanel _gameCanvas = null!;
    private System.Windows.Forms.Timer _gameTimer = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _soundManager?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _gameCanvas = new GameCanvasPanel();
        _gameTimer = new System.Windows.Forms.Timer(components);

        SuspendLayout();

        // Oyun tuvali
        _gameCanvas.Location = new Point(0, 0);
        _gameCanvas.Name = "_gameCanvas";
        _gameCanvas.Size = new Size(GameConfig.CanvasWidth, GameConfig.CanvasHeight);
        _gameCanvas.TabIndex = 0;
        _gameCanvas.Paint += GameCanvas_Paint;

        // Oyun dongusu zamanlayicisi
        _gameTimer.Interval = GameConfig.TimerIntervalMs;
        _gameTimer.Tick += GameTimer_Tick;

        // Ana form
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(GameConfig.CanvasWidth, GameConfig.CanvasHeight);
        Controls.Add(_gameCanvas);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        KeyPreview = true;
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "YMH219 - Flappy Bird";

        ResumeLayout(false);
    }
}
