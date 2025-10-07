using System;
using System.Collections.Generic;
using System.IO;

namespace RestoreWorkstation
{
    public static class Logger
    {
        private static readonly List<string> _logEntries = new();
        private static string? _logFilePath;
        private static Action<string>? _onLog;

        public static void Configure(string? logFilePath = null, Action<string>? onLog = null)
        {
            _logFilePath = logFilePath;
            _onLog = onLog;
        }

        public static void Log(string message)
        {
            string timestamped = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}";
            _logEntries.Add(timestamped);

            _onLog?.Invoke(timestamped);

            if (!string.IsNullOrWhiteSpace(_logFilePath))
            {
                try
                {
                    File.AppendAllText(_logFilePath, timestamped + Environment.NewLine);
                }
                catch (IOException ex)
                {
                    _onLog?.Invoke($"[Logger] Failed to write to log file: {ex.Message}");
                }
            }
        }

        public static IEnumerable<string> GetLogEntries() => _logEntries;

        public static void FlushToFile()
        {
            if (!string.IsNullOrWhiteSpace(_logFilePath))
            {
                try
                {
                    File.WriteAllLines(_logFilePath, _logEntries);
                }
                catch (IOException ex)
                {
                    _onLog?.Invoke($"[Logger] Flush failed: {ex.Message}");
                }
            }
        }
    }
}