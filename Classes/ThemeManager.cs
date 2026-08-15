using Chronos.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

namespace Chronos.Classes
{
    public static class ThemeManager
    {
        private static string _currentTheme = Settings.Default.Theme;

        public static string CurrentTheme
            => _currentTheme;

        public static event Action? ThemeChanged;

        public static void SetTheme(string themeName)
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries;

            for (int i = dictionaries.Count - 2; i >= 0; i--)
            {
                var src = dictionaries[i].Source?.OriginalString;

                if (src != null && src.Contains("/Themes/"))
                    dictionaries.RemoveAt(i);
            }

            var dict = new ResourceDictionary
            {
                Source = new Uri($"Resources/Themes/{themeName}.xaml", UriKind.Relative)
            };

            dictionaries.Add(dict);

            _currentTheme = themeName;
            ThemeChanged?.Invoke();
        }

        /// <summary>
        /// Opens a file dialog pre-set to the Themes folder so the user can select a theme file.
        /// </summary>
        public static void SelectThemeFromFile()
        {
            string themesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Themes");

            if (!Directory.Exists(themesFolderPath))
            {
                Directory.CreateDirectory(themesFolderPath);
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = themesFolderPath,
                Filter = "XAML Theme Files (*.xaml)|*.xaml|All Files (*.*)|*.*",
                Title = "Select a Theme"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string themeName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                SetTheme(themeName);
            }
        }
    }
}