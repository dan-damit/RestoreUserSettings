using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;

namespace RestoreUserSettings
{
    public partial class MainWindow : Window
    {
        private Logger _logger;
        private RestoreManager _restoreManager;

        public MainWindow()
        {
            InitializeComponent();
            _logger = new Logger(null, Log);
            _restoreManager = new RestoreManager(_logger);
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Registry Files (*.reg)|*.reg",
                Title = "Select HKCU Backup File"
            };

            if (dialog.ShowDialog() == true)
            {
                FilePathBox.Text = dialog.FileName;
                _logger.Log($"📂 Selected file: {dialog.FileName}");
            }
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            string path = FilePathBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.Log("⚠ Please select a registry file first.");
                return;
            }

            _logger.Log("🔄 Starting registry restore...");
            bool success = _restoreManager.RestoreHKCU(path);

            if (success)
                _logger.Log("✅ Restore completed.");
            else
                _logger.Log("❌ Restore failed.");
        }

        private void Log(string message)
        {
            LogBox.AppendText(message + "\n");
            LogBox.ScrollToEnd();
        }
    }
}