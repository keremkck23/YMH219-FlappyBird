using System.Media;

namespace FlappyBird.Services;

/// <summary>
/// Harici ses dosyasi gerektirmeden bellekte basit WAV efektleri uretip calar.
/// </summary>
public class SoundManager : IDisposable
{
    private readonly MemoryStream _jumpStream;
    private readonly MemoryStream _scoreStream;
    private readonly MemoryStream _hitStream;

    public bool IsMuted { get; private set; }

    public SoundManager()
    {
        _jumpStream = CreateToneWave(520, 720, 75, 0.22);
        _scoreStream = CreateToneWave(880, 1180, 110, 0.25);
        _hitStream = CreateToneWave(240, 110, 170, 0.30);
    }

    public void ToggleMute()
    {
        IsMuted = !IsMuted;
    }

    public void PlayJump() => PlayStream(_jumpStream);
    public void PlayScore() => PlayStream(_scoreStream);
    public void PlayHit() => PlayStream(_hitStream);

    private void PlayStream(MemoryStream stream)
    {
        if (IsMuted)
        {
            return;
        }

        try
        {
            stream.Position = 0;
            using var player = new SoundPlayer(stream);
            player.Play();
        }
        catch
        {
            // Ses aygiti yoksa sessizce devam et.
        }
    }

    private static MemoryStream CreateToneWave(double startFreq, double endFreq, int durationMs, double volume)
    {
        const int sampleRate = 22050;
        int sampleCount = sampleRate * durationMs / 1000;
        int dataLength = sampleCount * 2;

        var stream = new MemoryStream(44 + dataLength);
        using var writer = new BinaryWriter(stream, System.Text.Encoding.ASCII, leaveOpen: true);

        // RIFF WAV basligi
        writer.Write("RIFF".ToCharArray());
        writer.Write(36 + dataLength);
        writer.Write("WAVE".ToCharArray());
        writer.Write("fmt ".ToCharArray());
        writer.Write(16);
        writer.Write((short)1); // PCM
        writer.Write((short)1); // Mono
        writer.Write(sampleRate);
        writer.Write(sampleRate * 2);
        writer.Write((short)2);
        writer.Write((short)16);
        writer.Write("data".ToCharArray());
        writer.Write(dataLength);

        double phase = 0;
        for (int i = 0; i < sampleCount; i++)
        {
            double progress = (double)i / sampleCount;
            double freq = startFreq + (endFreq - startFreq) * progress;
            double envelope = 1.0 - progress;
            phase += 2.0 * Math.PI * freq / sampleRate;
            short sample = (short)(Math.Sin(phase) * short.MaxValue * volume * envelope);
            writer.Write(sample);
        }

        stream.Position = 0;
        return stream;
    }

    public void Dispose()
    {
        _jumpStream.Dispose();
        _scoreStream.Dispose();
        _hitStream.Dispose();
    }
}
