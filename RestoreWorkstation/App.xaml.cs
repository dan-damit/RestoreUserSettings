using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.Versioning;
using System.Windows;

namespace RestoreWorkstation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        [SupportedOSPlatform("windows")]
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string logPath = Path.Combine(
                Path.GetTempPath(),
                "RestoreWorkstation",
                $"RestoreLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            );

            Logger.Init(logPath);
            Logger.Log("🟢 Logger initialized at startup.");
        }

    }
}