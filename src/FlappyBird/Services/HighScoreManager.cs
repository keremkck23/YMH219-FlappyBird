namespace FlappyBird.Services;

/// <summary>
/// En yuksek skoru yerel dosya sisteminde kalici olarak saklar.
/// </summary>
public class HighScoreManager
{
    private readonly string _filePath;

    public HighScoreManager()
    {
        string appFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "YMH219-FlappyBird");
        _filePath = Path.Combine(appFolder, "highscore.txt");
    }

    public int Load()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string content = File.ReadAllText(_filePath).Trim();
                if (int.TryParse(content, out int score) && score >= 0)
                {
                    return score;
                }
            }
        }
        catch
        {
            // Dosya okuma hatasi oyunu engellememelidir.
        }

        return 0;
    }

    public void Save(int highScore)
    {
        try
        {
            string? directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(_filePath, highScore.ToString());
        }
        catch
        {
            // Dosya yazma hatasi oyunu engellememelidir.
        }
    }
}
