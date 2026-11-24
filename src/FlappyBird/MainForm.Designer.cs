#nullable enable
using FlappyBird.Controls;
using FlappyBird.Core;

namespace FlappyBird;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private Panel _topBarPanel = null!;
    private Label _lblScore = null!;
    private Label _lblHighScore = null!;
    private Label _lblSpeed = null!;
    private Button _btnNewGame = null!;
    private Button _btnPause = null!;
    private Button _btnMute = null!;
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
        _topBarPanel = new Panel();
        _lblScore = new Label();
        _lblHighScore = new Label();
        _lblSpeed = new Label();
        _btnNewGame = new Button();
        _btnPause = new Button();
        _btnMute = new Button();
        _gameCanvas = new GameCanvasPanel();
        _gameTimer = new System.Windows.Forms.Timer(components);

        _topBarPanel.SuspendLayout();
        SuspendLayout();

        // Ust kontrol cubugu
        _topBarPanel.BackColor = Color.FromArgb(24, 34, 48);
        _topBarPanel.Size = new Size(GameConfig.CanvasWidth, GameConfig.TopBarHeight);
        _topBarPanel.Dock = DockStyle.Top;
        _topBarPanel.Name = "_topBarPanel";
        _topBarPanel.TabIndex = 0;

        _lblScore.AutoSize = true;
        _lblScore.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        _lblScore.ForeColor = Color.White;
        _lblScore.Location = new Point(12, 15);
        _lblScore.Name = "_lblScore";
        _lblScore.Text = "Skor: 0";

        _lblHighScore.AutoSize = true;
        _lblHighScore.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        _lblHighScore.ForeColor = Color.FromArgb(255, 214, 74);
        _lblHighScore.Location = new Point(88, 15);
        _lblHighScore.Name = "_lblHighScore";
        _lblHighScore.Text = "En Yuksek: 0";

        _lblSpeed.AutoSize = true;
        _lblSpeed.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
        _lblSpeed.ForeColor = Color.FromArgb(165, 212, 240);
        _lblSpeed.Location = new Point(196, 16);
        _lblSpeed.Name = "_lblSpeed";
        _lblSpeed.Text = "Hiz: 3.2";

        _btnNewGame.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _btnNewGame.BackColor = Color.FromArgb(46, 160, 67);
        _btnNewGame.FlatAppearance.BorderSize = 0;
        _btnNewGame.FlatStyle = FlatStyle.Flat;
        _btnNewGame.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        _btnNewGame.ForeColor = Color.White;
        _btnNewGame.Location = new Point(268, 10);
        _btnNewGame.Name = "_btnNewGame";
        _btnNewGame.Size = new Size(80, 28);
        _btnNewGame.TabStop = false;
        _btnNewGame.Text = "Yeni Oyun";
        _btnNewGame.UseVisualStyleBackColor = false;
        _btnNewGame.Click += BtnNewGame_Click;

        _btnPause.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _btnPause.BackColor = Color.FromArgb(56, 118, 186);
        _btnPause.FlatAppearance.BorderSize = 0;
        _btnPause.FlatStyle = FlatStyle.Flat;
        _btnPause.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        _btnPause.ForeColor = Color.White;
        _btnPause.Location = new Point(354, 10);
        _btnPause.Name = "_btnPause";
        _btnPause.Size = new Size(64, 28);
        _btnPause.TabStop = false;
        _btnPause.Text = "Duraklat";
        _btnPause.UseVisualStyleBackColor = false;
        _btnPause.Click += BtnPause_Click;

        _btnMute.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        _btnMute.BackColor = Color.FromArgb(72, 84, 102);
        _btnMute.FlatAppearance.BorderSize = 0;
        _btnMute.FlatStyle = FlatStyle.Flat;
        _btnMute.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        _btnMute.ForeColor = Color.White;
        _btnMute.Location = new Point(424, 10);
        _btnMute.Name = "_btnMute";
        _btnMute.Size = new Size(46, 28);
        _btnMute.TabStop = false;
        _btnMute.Text = "Ses";
        _btnMute.UseVisualStyleBackColor = false;
        _btnMute.Click += BtnMute_Click;

        _topBarPanel.Controls.Add(_lblScore);
        _topBarPanel.Controls.Add(_lblHighScore);
        _topBarPanel.Controls.Add(_lblSpeed);
        _topBarPanel.Controls.Add(_btnNewGame);
        _topBarPanel.Controls.Add(_btnPause);
        _topBarPanel.Controls.Add(_btnMute);

        // Oyun tuvali
        _gameCanvas.Dock = DockStyle.Fill;
        _gameCanvas.Name = "_gameCanvas";
        _gameCanvas.TabIndex = 1;
        _gameCanvas.Paint += GameCanvas_Paint;
        _gameCanvas.MouseDown += GameCanvas_MouseDown;
        _gameCanvas.Resize += (_, _) => _gameCanvas.Invalidate();

        // Oyun dongusu zamanlayicisi
        _gameTimer.Interval = GameConfig.TimerIntervalMs;
        _gameTimer.Tick += GameTimer_Tick;

        // Ana form
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(GameConfig.CanvasWidth, GameConfig.TopBarHeight + GameConfig.CanvasHeight);
        Controls.Add(_gameCanvas);
        Controls.Add(_topBarPanel);
        FormBorderStyle = FormBorderStyle.Sizable;
        KeyPreview = true;
        MaximizeBox = true;
        MinimumSize = new Size(GameConfig.CanvasWidth + 16, GameConfig.TopBarHeight + GameConfig.CanvasHeight + 39);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "YMH219 - Flappy Bird";
        KeyDown += MainForm_KeyDown;
        KeyUp += MainForm_KeyUp;
        PreviewKeyDown += MainForm_PreviewKeyDown;

        _topBarPanel.ResumeLayout(false);
        _topBarPanel.PerformLayout();
        ResumeLayout(false);
    }
}
