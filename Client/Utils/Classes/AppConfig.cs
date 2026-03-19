namespace Client.Utils.Classes;

public static class AppConfig
{
    /// <summary>
    /// Indicates whether the application is running in Demo Mode (e.g., no backend connection).
    /// </summary>
    public static bool IsDemoMode { get; set; } = false;
}
