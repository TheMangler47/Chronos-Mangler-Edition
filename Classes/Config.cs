using System.Reflection;

namespace Chronos.Classes
{
    public static class Config
    {
        private static readonly Assembly CurrentAssembly = Assembly.GetExecutingAssembly();

        public static string AppName { get; } =
            CurrentAssembly.GetName().Name ?? "Chronos";

        public static string AppDescription { get; } =
            "A simple yet powerful text editor, Modified by TheMangler47";

        public static string AppVersion { get; } =
            CurrentAssembly.GetName().Version?.ToString(3) ?? "1.0.1";

        public static string MainRepositoryUrl { get; } =
            "https://github.com/nam3lol/Chronos";

        public static string ForkRepositoryUrl { get; } =
            "https://github.com/TheMangler47/Chronos-Mangler-Edition";

        public static string[] GitHubRepositories { get; } =
        [
            MainRepositoryUrl,
            ForkRepositoryUrl
        ];
    }
}
