using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Chronos
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                checkBox_AlwaysOnTop.IsChecked = mainWin.Topmost;

            }
            this.MouseDown += SettingsWindow_MouseDown;
        }

        private void checkBox_AlwaysOnTop_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                mainWin.Topmost = checkBox_AlwaysOnTop.IsChecked ?? false;
            }
        }

        private void SettingsWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void button_SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWin)
            {
                mainWin.Topmost = checkBox_AlwaysOnTop.IsChecked ?? false;
            }

            this.Close();
        }

        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void comboBox_Theme_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
        }
    }
}