using Domain.Interfaces;
using System;
using System.IO;
using System.Text;

namespace Infrastructure
{
    public class FileLogger : ILogger
    {
        private readonly string logFilePath;
        private static readonly object fileLock = new object();

        public FileLogger(string logFilePath)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
                throw new ArgumentException("Log file path cannot be empty.", nameof(logFilePath));

            this.logFilePath = logFilePath;

            var directory = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public void Info(string message)
            => Write("INFO", message);

        public void Warning(string message)
            => Write("WARN", message);

        public void Error(string message)
            => Write("ERROR", message);

        public void Error(Exception exception, string message = null)
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(message))
                sb.AppendLine(message);

            sb.AppendLine(exception.ToString());
            sb.AppendLine(exception.StackTrace);

            Write("ERROR", sb.ToString());
        }

        private void Write(string level, string message)
        {
            var logLine =
                $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] " +
                $"[{level}] " +
                $"{message}";

            lock (fileLock)
            {
                File.AppendAllText(logFilePath, logLine + Environment.NewLine);
            }
        }
    }
}
