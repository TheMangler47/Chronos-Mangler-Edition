using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Chronos
{
    /// <summary>
    /// Interaction logic for Credits.xaml
    /// </summary>
    public partial class Credits : Window
    {
        public Credits()
        {
            InitializeComponent();

            this.MouseDown += SettingsWindow_MouseDown;
        }
        private void SettingsWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_GitHub_Click(object sender, RoutedEventArgs e)
        {
            string[] repositories = new[]
            {
                "https://github.com/nam3lol/Chronos",
                "https://github.com/TheMangler47/Chronos-Mangler-Edition"
            };

            foreach (var url in repositories)
            {
                OpenUrl(url);
            }
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show($"Could not open browser for:\n{url}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
