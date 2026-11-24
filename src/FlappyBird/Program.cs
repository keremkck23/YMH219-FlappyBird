namespace FlappyBird;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        if (args.Length >= 2 && string.Equals(args[0], "--capture-screenshots", StringComparison.OrdinalIgnoreCase))
        {
            using var form = new MainForm();
            form.CaptureDocumentationScreenshots(args[1]);
            return;
        }

        Application.Run(new MainForm());
    }
}
