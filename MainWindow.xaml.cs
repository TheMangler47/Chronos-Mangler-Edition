using Chronos.Classes;
using Chronos.Properties;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Chronos
{
    public partial class MainWindow : Window
    {
        public bool LoadSavedEditorContent { get; set; } = true;

        private Geometry? geometry_Maximize;
        private Geometry? geometry_Restore;
        private Geometry? geometry_SidebarEnabled;
        private Geometry? geometry_SidebarDisabled;
        private Geometry? geometry_BottombarEnabled;
        private Geometry? geometry_BottombarDisabled;
        private Geometry? geometry_LightMode;
        private Geometry? geometry_DarkMode;
        private Geometry? geometry_RedMode;
        private Geometry? geometry_BlueMode;
        private Geometry? geometry_PinkMode;

        private Geometry? GetGeometry(string resourceKey)
        {
            return Application.Current.TryFindResource(resourceKey) as Geometry;
        }

        private Geometry? GeometryMaximize => geometry_Maximize ??= GetGeometry("Maximize");
        private Geometry? GeometryRestore => geometry_Restore ??= GetGeometry("Restore");
        private Geometry? GeometrySidebarEnabled => geometry_SidebarEnabled ??= GetGeometry("Sidebar_Enabled");
        private Geometry? GeometrySidebarDisabled => geometry_SidebarDisabled ??= GetGeometry("Sidebar_Disabled");
        private Geometry? GeometryBottombarEnabled => geometry_BottombarEnabled ??= GetGeometry("Bottombar_Enabled");
        private Geometry? GeometryBottombarDisabled => geometry_BottombarDisabled ??= GetGeometry("Bottombar_Disabled");
        private Geometry? GeometryLightMode => geometry_LightMode ??= GetGeometry("LightMode");
        private Geometry? GeometryDarkMode => geometry_DarkMode ??= GetGeometry("DarkMode");
        private Geometry? GeometryRedMode => geometry_RedMode ??= GetGeometry("RedMode");
        private Geometry? GeometryBlueMode => geometry_BlueMode ??= GetGeometry("BlueMode");
        private Geometry? GeometryPinkMode => geometry_PinkMode ??= GetGeometry("PinkMode");

        public MainWindow() =>
            InitializeComponent();

        public void LoadSettings()
        {
            ToggleSidebar(Settings.Default.Sidebar);
            ToggleBottombar(Settings.Default.Bottombar);
            ToggleLineNumbers(Settings.Default.LineNumbers);

            ThemeManager.ThemeChanged += UpdateThemeToggleUI;

            string savedTheme = string.IsNullOrWhiteSpace(Settings.Default.Theme) ? "Dark" : Settings.Default.Theme;
            ThemeManager.SetTheme(savedTheme);

            Topmost = Settings.Default.Topmost;
            avalonEdit_TextEditor.FontFamily = new FontFamily(Settings.Default.EditorFont);
        }

        private void UpdateThemeToggleUI()
        {
            string currentTheme = ThemeManager.CurrentTheme;
            menuItem_ChangeTheme.Header = $"Theme ({currentTheme})";

            switch (currentTheme.ToLowerInvariant())
            {
                case "light":
                    menuItem_ChangeTheme.Tag = GeometryLightMode;
                    break;
                case "red":
                    menuItem_ChangeTheme.Tag = GeometryRedMode;
                    break;
                case "blue":
                    menuItem_ChangeTheme.Tag = GeometryBlueMode; 
                    break;
                case "pink":
                    menuItem_ChangeTheme.Tag = GeometryPinkMode;
                    break;
                case "dark":
                default:
                    menuItem_ChangeTheme.Tag = GeometryDarkMode;
                    break;
            }
        }

        public void SaveSettings()
        {
            Settings.Default.Sidebar = border_Sidebar.Visibility == Visibility.Visible;
            Settings.Default.Bottombar = border_Bottombar.Visibility == Visibility.Visible;
            Settings.Default.LineNumbers = avalonEdit_TextEditor.ShowLineNumbers;
            Settings.Default.Topmost = Topmost;
            Settings.Default.EditorFont = avalonEdit_TextEditor.FontFamily.Source;
            Settings.Default.SavedEditorContent = avalonEdit_TextEditor.Text;
            Settings.Default.Theme = ThemeManager.CurrentTheme;
            Settings.Default.Save();
        }

        public void ToggleMaximized(bool maximized)
        {
            if (maximized)
            {
                WindowState = WindowState.Maximized;
                path_Maximize.Data = GeometryRestore;
            }
            else
            {
                WindowState = WindowState.Normal;
                path_Maximize.Data = GeometryMaximize;
            }
        }

        private GridLength _sidebarWidth = new(194);

        public void ToggleSidebar(bool enabled)
        {
            if (!enabled && SidebarColumn.Width.Value > 0)
            {
                _sidebarWidth = SidebarColumn.Width;
            }

            if (enabled && (_sidebarWidth.Value <= 0 || double.IsNaN(_sidebarWidth.Value)))
            {
                _sidebarWidth = new GridLength(220); 
            }

            border_Sidebar.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
            SidebarColumn.MinWidth = enabled ? 194 : 0;
            SidebarColumn.Width = enabled ? _sidebarWidth : new GridLength(0);

            SplitterColumn.Width = enabled ? new GridLength(5) : new GridLength(0);

            if (menuItem_ToggleSidebar != null)
                menuItem_ToggleSidebar.IsChecked = enabled;

            if (path_Sidebar != null)
                path_Sidebar.Data = enabled ? GeometrySidebarEnabled : GeometrySidebarDisabled;
        }

        public void ToggleBottombar(bool enabled)
        {
            border_Bottombar.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;

            if (path_Bottombar != null)
                path_Bottombar.Data = enabled ? GeometryBottombarEnabled : GeometryBottombarDisabled;

            if (menuItem_ToggleBottombar != null)
                menuItem_ToggleBottombar.IsChecked = enabled;
        }

        public void ToggleLineNumbers(bool enabled)
        {
            avalonEdit_TextEditor.ShowLineNumbers = enabled;
            menuItem_ToggleLineNumbers.IsChecked = enabled;
        }

        private void TextEditor_TextChanged(object? sender, EventArgs e) => 
            UpdateUndoRedoState();

        private void Drag(object sender, MouseButtonEventArgs e) => 
            DragMove();

        private void OpenFile()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chronos - Open File",
                Filter = "Lua Script (*.lua)|*.lua|Normal Text File (*.txt)|*.txt|Markdown File (*.md;*.markdown)|*.md;*.markdown|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string contents = File.ReadAllText(dialog.FileName);
                    avalonEdit_TextEditor.Text = contents;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error opening file: {ex.Message}", "Chronos", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveFile()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Chronos - Save File",
                Filter = "Lua Script (*.lua)|*.lua|Text File (*.txt)|*.txt|Markdown File (*.md;*.markdown)|*.md;*.markdown|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dialog.FileName, avalonEdit_TextEditor.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Chronos", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateUndoRedoState()
        {
            bool canUndo = avalonEdit_TextEditor.CanUndo;
            bool canRedo = avalonEdit_TextEditor.CanRedo;

            button_Undo.IsEnabled = canUndo;
            button_Redo.IsEnabled = canRedo;

            menuItem_Undo.IsEnabled = canUndo;
            menuItem_Redo.IsEnabled = canRedo;
        }

        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            Credits creditsWindow = new Credits();
            //creditsWindow.Owner = this; 
            creditsWindow.ShowDialog(); 
        }

        public async void CloseWindow()
        {
            SaveSettings();
            AnimLib.Fade(this, 1, 0, 500);
            await Task.Delay(500);
            Close();
        }

        private async void buttonClick_WindowControls(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            switch (button.Name)
            {
                case "button_Close":
                    CloseWindow();
                    break;
                case "button_Maximize":
                    ToggleMaximized(WindowState != WindowState.Maximized);
                    break;
                case "button_Minimize":
                    WindowState = WindowState.Minimized;
                    break;
            }
        }

        private void buttonClick_ToolbarControlsRight(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            switch (button.Name)
            {
                case "button_ToggleSidebar":
                    ToggleSidebar(border_Sidebar.Visibility != Visibility.Visible);
                    break;
                case "button_ToggleBottombar":
                    ToggleBottombar(border_Bottombar.Visibility != Visibility.Visible);
                    break;
                case "button_Settings":
                    SettingsWindow settingsWindow = new SettingsWindow();
                    settingsWindow.Owner = this; // Sets the main window as the owner
                    settingsWindow.ShowDialog(); // Opens as a modal window (or use settingsWindow.Show() for non-modal)
                    break;
            }
        }

        private void buttonClick_ToolbarControlsLeft(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;

            switch (button.Name)
            {
                case "button_OpenFile":
                    OpenFile();
                    UpdateUndoRedoState();
                    break;
                case "button_SaveFile":
                    SaveFile();
                    UpdateUndoRedoState();
                    break;
                case "button_Cut":
                    avalonEdit_TextEditor.Cut();
                    break;
                case "button_Copy":
                    avalonEdit_TextEditor.Copy();
                    break;
                case "button_Paste":
                    avalonEdit_TextEditor.Paste();
                    break;
                case "button_Delete":
                    avalonEdit_TextEditor.Delete();
                    break;
                case "button_Undo":
                    avalonEdit_TextEditor.Undo();
                    UpdateUndoRedoState();
                    break;
                case "button_Redo":
                    avalonEdit_TextEditor.Redo();
                    UpdateUndoRedoState();
                    break;
            }
        }

        private void menuItemClick_File(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;

            switch (menuItem.Name)
            {
                case "menuItem_NewWindow":
                    SaveSettings();
                    var window = new MainWindow { LoadSavedEditorContent = false };
                    window.Show();
                    break;
                case "menuItem_OpenFile":
                    OpenFile();
                    UpdateUndoRedoState();
                    break;
                case "menuItem_SaveFile":
                    SaveFile();
                    UpdateUndoRedoState();
                    break;
                case "menuItem_Exit":
                    CloseWindow();
                    break;
            }
        }
        private void menuItemClick_Edit(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;

            switch (menuItem.Name)
            {
                case "menuItem_Cut":
                    avalonEdit_TextEditor.Cut();
                    break;
                case "menuItem_Copy":
                    avalonEdit_TextEditor.Copy();
                    break;
                case "menuItem_Paste":
                    avalonEdit_TextEditor.Paste();
                    break;
                case "menuItem_Delete":
                    avalonEdit_TextEditor.Delete();
                    break;
                case "menuItem_Undo":
                    avalonEdit_TextEditor.Undo();
                    UpdateUndoRedoState();
                    break;
                case "menuItem_Redo":
                    avalonEdit_TextEditor.Redo();
                    UpdateUndoRedoState();
                    break;
                case "menuItem_SearchOnline":
                    string query = !string.IsNullOrWhiteSpace(avalonEdit_TextEditor.SelectedText)
                        ? avalonEdit_TextEditor.SelectedText
                        : avalonEdit_TextEditor.Text;

                    if (string.IsNullOrWhiteSpace(query))
                    {
                        MessageBox.Show("Please select or enter text to search online.", "Chronos", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }

                    try
                    {
                        string encodedQuery = Uri.EscapeDataString(query.Trim());
                        string url = $"https://www.google.com/search?q={encodedQuery}";

                        Process.Start(new ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unable to open Web Browser: {ex.Message}", "Chronos", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    break;
            }
        }
        private void menuItemClick_View(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;

            switch (menuItem.Name)
            {
                case "menuItem_ToggleSidebar":
                    ToggleSidebar(border_Sidebar.Visibility != Visibility.Visible);
                    break;
                case "menuItem_ToggleBottombar":
                    ToggleBottombar(border_Bottombar.Visibility != Visibility.Visible);
                    break;
                case "menuItem_ToggleLineNumbers":
                    ToggleLineNumbers(!avalonEdit_TextEditor.ShowLineNumbers);
                    break;
                case "menuItem_ChangeTheme":
                    ThemeManager.SelectThemeFromFile();
                    break;
            }
        }

        public async Task StartSplashAnimAsync()
        {
            const int WindowFadeInMs = 500;
            const int SplashHoldMs = 2000;
            const int TransitionMs = 600;

            grid_Splash.Margin = new Thickness(0);
            grid_Splash.Visibility = Visibility.Visible;

            AnimLib.Fade(this, 0, 1, WindowFadeInMs);
            AnimLib.Fade(grid_Splash, 0, 1, WindowFadeInMs);
            await Task.Delay(WindowFadeInMs + SplashHoldMs);

            grid_Splash.IsHitTestVisible = false; 
            grid_BorderGrid.Visibility = Visibility.Visible;

            AnimLib.Fade(grid_Splash, 1, 0, TransitionMs);
            AnimLib.Fade(grid_BorderGrid, 0, 1, TransitionMs);

            await Task.Delay(TransitionMs);

            grid_Splash.Visibility = Visibility.Collapsed;
            grid_Splash.Margin = new Thickness(999);
        }

        private void InitializeUIState()
        {
            Opacity = 0;
            grid_BorderGrid.Opacity = 0;
            grid_BorderGrid.Visibility = Visibility.Collapsed;
            grid_Splash.Opacity = 0;
        }

        private void InitializeEditor()
        {
            avalonEdit_TextEditor.TextChanged += TextEditor_TextChanged;

            if (LoadSavedEditorContent)
            {
                string savedContent = Settings.Default.SavedEditorContent;
                if (!string.IsNullOrEmpty(savedContent))
                    avalonEdit_TextEditor.Text = savedContent;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeUIState();
            InitializeEditor();
            LoadSettings();
            UpdateUndoRedoState();

            await StartSplashAnimAsync();
        }
    }
}