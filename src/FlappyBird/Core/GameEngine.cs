using FlappyBird.Models;
using FlappyBird.Services;

namespace FlappyBird.Core;

/// <summary>
/// Oyunun tum durumunu, nesnelerini, hizini ve skorunu yoneten merkezi motor sinifi.
/// </summary>
public class GameEngine
{
    private readonly Random _random = new();
    private readonly HighScoreManager _highScoreManager;
    private readonly SoundManager _soundManager;

    public Bird Bird { get; }
    public Ground Ground { get; }
    public List<PipeObstacle> Pipes { get; }
    public List<Cloud> Clouds { get; }
    public List<FloatingText> FloatingTexts { get; }

    public GameStatus Status { get; private set; }
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public float CurrentSpeed { get; private set; }
    public int CurrentPipeGap { get; private set; }
    public int PassedPipeCount { get; private set; }
    public bool IsMuted => _soundManager.IsMuted;

    public GameEngine(HighScoreManager highScoreManager, SoundManager soundManager)
    {
        _highScoreManager = highScoreManager;
        _soundManager = soundManager;

        Bird = new Bird();
        Ground = new Ground();
        Pipes = new List<PipeObstacle>();
        Clouds = CreateInitialClouds();
        FloatingTexts = new List<FloatingText>();

        HighScore = _highScoreManager.Load();
        ResetToReady();
    }

    public void ResetToReady()
    {
        Bird.Reset();
        Ground.Reset();
        FloatingTexts.Clear();

        Score = 0;
        PassedPipeCount = 0;
        CurrentSpeed = GameConfig.InitialPipeSpeed;
        CurrentPipeGap = GameConfig.InitialPipeGap;
        Status = GameStatus.Ready;

        InitializePipes();
    }

    private void InitializePipes()
    {
        Pipes.Clear();
        float startX = GameConfig.CanvasWidth + 90;

        for (int i = 0; i < GameConfig.PipeCount; i++)
        {
            int gapY = GenerateRandomGapY(CurrentPipeGap);
            Pipes.Add(new PipeObstacle(startX + i * GameConfig.PipeHorizontalSpacing, gapY, CurrentPipeGap));
        }
    }

    private int GenerateRandomGapY(int gapHeight)
    {
        int minGapY = GameConfig.MinPipeTopHeight;
        int maxGapY = GameConfig.PlayableHeight - gapHeight - GameConfig.MinPipeTopHeight;
        return _random.Next(minGapY, Math.Max(minGapY + 1, maxGapY));
    }

    private static List<Cloud> CreateInitialClouds()
    {
        return new List<Cloud>
        {
            new(45, 72, 1.0f, 0.22f),
            new(210, 128, 0.8f, 0.16f),
            new(360, 62, 1.15f, 0.26f),
            new(460, 165, 0.72f, 0.14f)
        };
    }

    public void ToggleMute()
    {
        _soundManager.ToggleMute();
    }

    /// <summary>
    /// Klavye veya fare ile ziplama komutu geldiginde oyun durumuna gore tepki verir.
    /// </summary>
    public void HandleJumpInput()
    {
        if (Status == GameStatus.Ready)
        {
            Status = GameStatus.Playing;
            Bird.Jump();
            _soundManager.PlayJump();
            return;
        }

        if (Status == GameStatus.Playing)
        {
            Bird.Jump();
            _soundManager.PlayJump();
        }
    }

    public void StartNewGame()
    {
        ResetToReady();
    }

    public void RestartAndPlay()
    {
        ResetToReady();
        Status = GameStatus.Playing;
        Bird.Jump();
        _soundManager.PlayJump();
    }

    public void TogglePause()
    {
        if (Status == GameStatus.Playing)
        {
            Status = GameStatus.Paused;
        }
        else if (Status == GameStatus.Paused)
        {
            Status = GameStatus.Playing;
        }
    }

    /// <summary>
    /// Her Timer tikinde (yaklasik 60 FPS) oyun dunyasini bir kare ilerletir.
    /// </summary>
    public void Update()
    {
        if (Status == GameStatus.Ready)
        {
            Bird.UpdateIdleHover();
            Ground.Update(CurrentSpeed * 0.55f);
            UpdateClouds(CurrentSpeed * 0.55f);
            return;
        }

        if (Status != GameStatus.Playing)
        {
            return;
        }

        Bird.ApplyGravity();
        Ground.Update(CurrentSpeed);
        UpdateClouds(CurrentSpeed);
        UpdatePipes();
        UpdateFloatingTexts();

        if (CheckCollisions())
        {
            TriggerGameOver();
        }
    }

    private void UpdateClouds(float speed)
    {
        foreach (Cloud cloud in Clouds)
        {
            cloud.Update(speed);
        }
    }

    private void UpdatePipes()
    {
        float rightmostX = Pipes.Max(p => p.X);

        foreach (PipeObstacle pipe in Pipes)
        {
            pipe.Move(CurrentSpeed);

            if (pipe.TryMarkPassed(Bird.X))
            {
                IncrementScore();
            }

            if (pipe.IsOffScreen)
            {
                rightmostX += GameConfig.PipeHorizontalSpacing;
                int newGapY = GenerateRandomGapY(CurrentPipeGap);
                pipe.Reposition(rightmostX, newGapY, CurrentPipeGap);
            }
        }
    }

    private void IncrementScore()
    {
        Score++;
        PassedPipeCount++;
        _soundManager.PlayScore();

        FloatingTexts.Add(new FloatingText("+1", Bird.X + Bird.Width * 0.5f, Bird.Y - 10));

        if (Score > HighScore)
        {
            HighScore = Score;
        }

        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        CurrentSpeed = Math.Min(
            GameConfig.InitialPipeSpeed + Score * GameConfig.SpeedIncreasePerPoint,
            GameConfig.MaxPipeSpeed);

        int gapReduction = (Score / 5) * GameConfig.GapDecreasePerFivePoints;
        CurrentPipeGap = Math.Max(GameConfig.InitialPipeGap - gapReduction, GameConfig.MinPipeGap);
    }

    private void UpdateFloatingTexts()
    {
        for (int i = FloatingTexts.Count - 1; i >= 0; i--)
        {
            FloatingTexts[i].Update();
            if (FloatingTexts[i].IsExpired)
            {
                FloatingTexts.RemoveAt(i);
            }
        }
    }

    private bool CheckCollisions()
    {
        RectangleF birdBounds = Bird.Bounds;

        // Zemin veya ust sinir carpismasi
        if (birdBounds.Bottom >= Ground.TopY || birdBounds.Top <= 0)
        {
            return true;
        }

        // Boru engelleriyle carpisma kontrolu
        foreach (PipeObstacle pipe in Pipes)
        {
            if (birdBounds.IntersectsWith(pipe.TopBounds) ||
                birdBounds.IntersectsWith(pipe.BottomBounds))
            {
                return true;
            }
        }

        return false;
    }

    private void TriggerGameOver()
    {
        Status = GameStatus.GameOver;
        Bird.ClampToGround();
        _soundManager.PlayHit();

        if (Score > HighScore)
        {
            HighScore = Score;
            _highScoreManager.Save(HighScore);
        }
    }

    /// <summary>
    /// Dokumantasyon ekran goruntuleri icin deterministik sahne durumu hazirlar.
    /// </summary>
    internal void SetupPreviewScene(GameStatus status, int score, int highScore)
    {
        ResetToReady();
        Status = status;
        Score = score;
        HighScore = Math.Max(highScore, score);
        UpdateDifficulty();

        if (status == GameStatus.Playing)
        {
            Pipes.Clear();
            Pipes.Add(new PipeObstacle(195, 165, CurrentPipeGap));
            Pipes.Add(new PipeObstacle(420, 225, CurrentPipeGap));
            Bird.Jump();
            Bird.ApplyGravity();
            Bird.ApplyGravity();
            FloatingTexts.Add(new FloatingText("+1", Bird.X + 26, Bird.Y - 14, 32));
        }
        else if (status == GameStatus.GameOver)
        {
            Pipes.Clear();
            Pipes.Add(new PipeObstacle(135, 175, CurrentPipeGap));
            Pipes.Add(new PipeObstacle(360, 240, CurrentPipeGap));
            for (int i = 0; i < 18; i++)
            {
                Bird.ApplyGravity();
            }
            Bird.ClampToGround();
        }
    }
}
