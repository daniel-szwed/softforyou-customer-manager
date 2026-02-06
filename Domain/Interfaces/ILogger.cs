using System;

namespace Domain.Interfaces
{
    public interface ILogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Error(Exception exception, string message = null);
    }
}
