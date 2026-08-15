using System.Reflection;

namespace Chronos.Classes
{
    public static class Config
    {
        public static readonly string? AppName =
            Assembly.GetExecutingAssembly().GetName().Name;

        public static readonly string AppDescription =
            "A simple yet powerful text editor, Modified by TheMangler47";

        public static readonly string? AppVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        public const string GitHubRepos =
            "https://github.com/nam3lol/Chronos" +
            "https://github.com/TheMangler47/Chronos-Mangler-Edition";
    }
}
