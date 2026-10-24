using System;
using System.IO;

namespace SmartGreeting.Core.Services
{
    public interface ILoggerService
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? ex = null);
        string LogFilePath { get; }
    }

    public class FileLoggerService : ILoggerService
    {
        private readonly string _logFilePath;
        private readonly object _lock = new object();

        public FileLoggerService(string? customLogPath = null)
        {
            if (string.IsNullOrWhiteSpace(customLogPath))
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string logDir = Path.Combine(localAppData, "SmartGreeting", "Logs");
                _logFilePath = Path.Combine(logDir, "smartgreeting.log");
            }
            else
            {
                _logFilePath = customLogPath!;
            }
        }

        public string LogFilePath => _logFilePath;

        public void LogInfo(string message) => WriteEntry("INFO", message);
        public void LogWarning(string message) => WriteEntry("WARN", message);
        public void LogError(string message, Exception? ex = null)
        {
            string full = ex != null ? $"{message} | Exception: {ex}" : message;
            WriteEntry("ERROR", full);
        }

        private void WriteEntry(string level, string message)
        {
            try
            {
                lock (_lock)
                {
                    string? dir = Path.GetDirectoryName(_logFilePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir!);
                    }

                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    string line = $"[{timestamp}] [{level}] {message}{Environment.NewLine}";
                    File.AppendAllText(_logFilePath, line);
                }
            }
            catch
            {
                // Never crash host application due to logging failure
            }
        }
    }
}
