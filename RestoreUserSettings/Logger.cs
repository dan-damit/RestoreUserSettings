using System;
using System.Collections.Generic;
using System.IO;

namespace RestoreUserSettings
{
    public class Logger
    {
        private readonly List<string> _logEntries = new();
        private readonly string? _logFilePath;
        private readonly Action<string>? _onLog;

        public Logger(string? logFilePath = null, Action<string>? onLog = null)
        {
            _logFilePath = logFilePath;
            _onLog = onLog;
        }

        public void Log(string message)
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
                catch
                {
                    // Silent fail — don't crash if log file is locked
                }
            }
        }

        public IEnumerable<string> GetLogEntries() => _logEntries;
    }
}