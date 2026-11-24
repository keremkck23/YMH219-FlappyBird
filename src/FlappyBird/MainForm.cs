using FlappyBird.Core;
using FlappyBird.Rendering;
using FlappyBird.Services;

namespace FlappyBird;

public partial class MainForm : Form
{
    private readonly HighScoreManager _highScoreManager;
    private readonly SoundManager _soundManager;
    private readonly GameEngine _engine;
    private readonly GameRenderer _renderer;
    private bool _isJumpKeyHeld;

    public MainForm()
    {
        _highScoreManager = new HighScoreManager();
        _soundManager = new SoundManager();
        _engine = new GameEngine(_highScoreManager, _soundManager);
        _renderer = new GameRenderer();

        InitializeComponent();
        LoadApplicationIcon();
        UpdateTopBar();
        _gameTimer.Start();
    }

    private void LoadApplicationIcon()
    {
        try
        {
            string iconPath = Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico");
            if (File.Exists(iconPath))
            {
                Icon = new Icon(iconPath);
            }
            else if (!string.IsNullOrEmpty(Application.ExecutablePath))
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
        }
        catch
        {
            // Ikon yuklenemezse varsayilan form ikonu kullanilir.
        }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Up || keyData == Keys.Space)
        {
            HandleJumpKeyDown();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void MainForm_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
    {
        if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Space)
        {
            e.IsInputKey = true;
        }
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Space:
            case Keys.Up:
            case Keys.W:
                HandleJumpKeyDown();
                e.Handled = true;
                break;

            case Keys.P:
                _engine.TogglePause();
                e.Handled = true;
                break;

            case Keys.R:
            case Keys.Enter:
                if (_engine.Status == GameStatus.GameOver || _engine.Status == GameStatus.Paused)
                {
                    _engine.RestartAndPlay();
                    e.Handled = true;
                }
                break;

            case Keys.M:
                _engine.ToggleMute();
                e.Handled = true;
                break;
        }
    }

    private void MainForm_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.Space or Keys.Up or Keys.W)
        {
            _isJumpKeyHeld = false;
        }
    }

    private void HandleJumpKeyDown()
    {
        if (_isJumpKeyHeld)
        {
            return;
        }

        _isJumpKeyHeld = true;
        if (_engine.Status == GameStatus.GameOver)
        {
            return;
        }

        _engine.HandleJumpInput();
    }

    private void GameCanvas_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        if (_engine.Status == GameStatus.GameOver)
        {
            _engine.RestartAndPlay();
        }
        else
        {
            _engine.HandleJumpInput();
        }
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        _engine.Update();
        UpdateTopBar();
        _gameCanvas.Invalidate();
    }

    private void GameCanvas_Paint(object? sender, PaintEventArgs e)
    {
        _renderer.Render(e.Graphics, _engine);
    }

    private void BtnNewGame_Click(object? sender, EventArgs e)
    {
        _engine.StartNewGame();
        UpdateTopBar();
        _gameCanvas.Invalidate();
        ActiveControl = null;
    }

    private void BtnPause_Click(object? sender, EventArgs e)
    {
        _engine.TogglePause();
        UpdateTopBar();
        _gameCanvas.Invalidate();
        ActiveControl = null;
    }

    private void BtnMute_Click(object? sender, EventArgs e)
    {
        _engine.ToggleMute();
        UpdateTopBar();
        ActiveControl = null;
    }

    private void UpdateTopBar()
    {
        _lblScore.Text = $"Skor: {_engine.Score}";
        _lblHighScore.Text = $"En Yuksek: {_engine.HighScore}";
        _lblSpeed.Text = $"Hiz: {_engine.CurrentSpeed:F1}";
        _btnPause.Text = _engine.Status == GameStatus.Paused ? "Devam" : "Duraklat";
        _btnMute.Text = _engine.IsMuted ? "Kapali" : "Ses";
        _btnMute.BackColor = _engine.IsMuted
            ? Color.FromArgb(164, 62, 62)
            : Color.FromArgb(72, 84, 102);
    }

    internal void CaptureDocumentationScreenshots(string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        _gameTimer.Stop();
        ShowInTaskbar = false;
        Opacity = 0;
        Show();
        Application.DoEvents();

        SaveSceneScreenshot(GameStatus.Ready, score: 0, highScore: 12, Path.Combine(outputDirectory, "start-screen.png"));
        SaveSceneScreenshot(GameStatus.Playing, score: 7, highScore: 12, Path.Combine(outputDirectory, "gameplay.png"));
        SaveSceneScreenshot(GameStatus.GameOver, score: 14, highScore: 14, Path.Combine(outputDirectory, "game-over.png"));

        Hide();
    }

    private void SaveSceneScreenshot(GameStatus status, int score, int highScore, string filePath)
    {
        _engine.SetupPreviewScene(status, score, highScore);
        UpdateTopBar();

        using var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
        using (var topBarBmp = new Bitmap(_topBarPanel.Width, _topBarPanel.Height))
        {
            _topBarPanel.DrawToBitmap(topBarBmp, new Rectangle(0, 0, _topBarPanel.Width, _topBarPanel.Height));
            using Graphics g = Graphics.FromImage(bitmap);
            g.DrawImage(topBarBmp, 0, 0);
            g.TranslateTransform(0, GameConfig.TopBarHeight);
            _renderer.Render(g, _engine);
        }

        bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
    }
}
