using System.Drawing;
using System.Text.Json;
using Photino.NET;

namespace WindowLocation;

public static class Program
{
    private static readonly string ConfigFile =
        Path.Combine(Path.GetDirectoryName(Environment.ProcessPath) ?? throw new InvalidOperationException(), "saved-state.json");

    private static PhotinoWindow? mainWindow;
    private static State windowState = new();

    [STAThread]
    public static void Main(string[] args)
    {
        mainWindow = new PhotinoWindow()
            .SetTitle($"Window Location Tester")
            .RegisterLocationChangedHandler(HandleLocationChangedDelegate)
            .RegisterSizeChangedHandler(HandleSizeChangedDelegate)
            .RegisterWindowClosingHandler(HandleWindowClosingDelegate)
            .SetUseOsDefaultSize(false)
            .SetResizable(true);

        RestoreState(mainWindow);
        
        Console.WriteLine($"Restored location: {mainWindow.Location.X}, {mainWindow.Location.Y}");

        mainWindow
            .Load("wwwroot/index.html")
            .WaitForClose();

        SaveState();
    }

    private static void HandleLocationChangedDelegate(object? sender, Point e)
    {
        if (mainWindow is null) return;
        windowState.X = mainWindow.Location.X;
        windowState.Y = mainWindow.Location.Y;
        Console.WriteLine($"{mainWindow.Location.X}, {mainWindow.Location.Y}");
    }

    private static void HandleSizeChangedDelegate(object? sender, Size e)
    {
        if (mainWindow is null) return;
        windowState.Width = mainWindow.Width;
        windowState.Height = mainWindow.Height;
    }

    private static bool HandleWindowClosingDelegate(object sender, EventArgs e)
    {
        if (mainWindow != null) windowState.Maximized = mainWindow.Maximized;
        return false;
    }

    private static void RestoreState(PhotinoWindow window)
    {
        if (File.Exists(ConfigFile))
        {
            var state = JsonSerializer.Deserialize<State>(File.ReadAllText(ConfigFile));

            if (state is not null)
            {
                var point = new Point(state.X, state.Y);
                window
                    .SetSize(state.Width, state.Height)
                    .SetLocation(point);
                if (state.Maximized)
                    window.SetMaximized(true);
                windowState = state;
                return;
            }
        }

        window
            .SetSize(800, 600)
            .Center();
    }

    private static void SaveState()
    {
        File.WriteAllText(ConfigFile, JsonSerializer.Serialize(windowState));
    }
}