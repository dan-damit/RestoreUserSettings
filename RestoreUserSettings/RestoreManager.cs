using System.Diagnostics;
using System.IO;

namespace RestoreUserSettings
{
    class RestoreManager
    {
        private readonly Logger _logger;

        public RestoreManager(Logger logger)
        {
            _logger = logger;
        }

        public bool RestoreHKCU(string regFilePath)
        {
            if (!File.Exists(regFilePath))
            {
                _logger.Log($"❌ Registry file not found: {regFilePath}");
                return false;
            }

            try
            {
                var psi = new ProcessStartInfo("reg.exe", $"import \"{regFilePath}\"")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc != null)
                {
                    proc.WaitForExit();
                    if (proc.ExitCode == 0)
                    {
                        _logger.Log($"✅ HKCU restored from: {regFilePath}");
                        return true;
                    }
                    else
                    {
                        _logger.Log($"⚠ reg.exe exited with code {proc.ExitCode}");
                        return false;
                    }
                }
                else
                {
                    _logger.Log("❌ Failed to start reg.exe");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"❌ Exception during restore: {ex.Message}");
                return false;
            }
        }
    }
}