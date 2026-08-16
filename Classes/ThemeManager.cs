using Chronos.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace Chronos.Classes
{
    using System;
    using System.IO;
    using System.Windows;
    using Microsoft.Win32;

    public static class ThemeManager
    {
        private static string _currentTheme = Settings.Default.Theme;

        public static string CurrentTheme => _currentTheme;

        public static event Action? ThemeChanged;

        public static void SetTheme(string themeName)
        {
            if (string.IsNullOrWhiteSpace(themeName)) return;

            var uri = new Uri($"pack://application:,,,/Resources/Themes/{themeName}.xaml", UriKind.Absolute);

            if (ApplyResourceDictionary(uri))
            {
                _currentTheme = themeName;
                ThemeChanged?.Invoke();
            }
        }

        public static void SelectThemeFromFile()
        {
            string themesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Themes");

            if (!Directory.Exists(themesFolderPath))
            {
                Directory.CreateDirectory(themesFolderPath);
            }

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = themesFolderPath,
                Filter = "XAML Theme Files (*.xaml)|*.xaml|All Files (*.*)|*.*",
                Title = "Select a Theme"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var fileUri = new Uri(openFileDialog.FileName, UriKind.Absolute);

                if (ApplyResourceDictionary(fileUri))
                {
                    _currentTheme = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    ThemeChanged?.Invoke();
                }
            }
        }

        private static bool ApplyResourceDictionary(Uri resourceUri)
        {

            ResourceDictionary newDict;
            try
            {
                newDict = new ResourceDictionary { Source = resourceUri };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load theme from {resourceUri}: {ex.Message}");
                return false;
            }

            var dictionaries = Application.Current.Resources.MergedDictionaries;

            for (int i = dictionaries.Count - 1; i >= 0; i--)
            {
                var src = dictionaries[i].Source?.OriginalString;

                if (src != null && src.Contains("/Themes/", StringComparison.OrdinalIgnoreCase))
                {
                    dictionaries.RemoveAt(i);
                }
            }

            dictionaries.Add(newDict);
            return true;
        }
    }
}