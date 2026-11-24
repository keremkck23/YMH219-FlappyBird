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

    public MainForm()
    {
        _highScoreManager = new HighScoreManager();
        _soundManager = new SoundManager();
        _engine = new GameEngine(_highScoreManager, _soundManager);
        _renderer = new GameRenderer();

        InitializeComponent();
        _gameTimer.Start();
    }

    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        _engine.Update();
        _gameCanvas.Invalidate();
    }

    private void GameCanvas_Paint(object? sender, PaintEventArgs e)
    {
        _renderer.Render(e.Graphics, _engine);
    }
}
