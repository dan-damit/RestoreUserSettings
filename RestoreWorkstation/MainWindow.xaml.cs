using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Input;

namespace RestoreWorkstation
{
    public partial class MainWindow : Window
    {
        [SupportedOSPlatform("windows")]

        private ObservableCollection<string> _logEntries = new ObservableCollection<string>();
        private RestoreManager? _restoreManager;

        public MainWindow()
        {
            InitializeComponent();
            LogBox.ItemsSource = _logEntries;
            Logger.LogMessageReceived += OnLogMessage;
        }

        // Enable window dragging from the title bar area
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Allow dragging the window when the left mouse button is held down
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        // Browse button click handler
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select Root Backup Data Folder",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = false
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                backupRootFolder.Text = dialog.SelectedPath;
                Logger.Log($"📂 Selected folder: {dialog.SelectedPath}");
            }
        }

        // Restore button click handler
        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            string path = backupRootFolder.Text.Trim();
            if (string.IsNullOrWhiteSpace(path))
            {
                Logger.Log("⚠ Please select a folder first.");
                return;
            }

            // UI preflight
            btnRestore.IsEnabled = false;
            this.Cursor = System.Windows.Input.Cursors.Wait;
            _logEntries.Clear();
            progressBar.Value = 0;
            _restoreManager ??= new RestoreManager();

            Logger.Log("🔄 Starting data restore...");
            bool success = _restoreManager.RestoreToUserProfile(path);
            bool regSuccess = _restoreManager.MergeRegistryKeys(path);

            if (success && regSuccess)
                Logger.Log("✅ Restore completed.");
            else if (!success && !regSuccess)
                Logger.Log("❌ Restore failed: file copy and registry merge both failed.");
            else if (!success)
                Logger.Log("⚠ Partial restore: file copy failed, registry merge succeeded.");
            else
                Logger.Log("⚠ Partial restore: file copy succeeded, registry merge failed.");

            // restore UI
            btnRestore.IsEnabled = true;
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        // Update log messages in the UI
        private void OnLogMessage(string message)
        {
            _logEntries.Add(message);
            if (LogBox.Items.Count > 0)
            {
                LogBox.ScrollIntoView(LogBox.Items[LogBox.Items.Count - 1]);
            }
        }

        // Close the application
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var prompt = new RebootPrompt();
            prompt.Owner = this;
            prompt.ShowDialog();
        }
    }
}