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
        _gameTimer.Start();
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
        _gameCanvas.Invalidate();
    }

    private void GameCanvas_Paint(object? sender, PaintEventArgs e)
    {
        _renderer.Render(e.Graphics, _engine);
    }
}
