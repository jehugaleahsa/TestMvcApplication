using System;

namespace ServiceInterfaces
{
    public interface ILogger
    {
        void Trace(string logName, string message, params object[] arguments);

        void Debug(string logName, string message, params object[] arguments);

        void Info(string logName, string message, params object[] arguments);

        void Error(string logName, string message, params object[] arguments);

        void Fatal(string logName, string message, params object[] arguments);

        void TraceException(string logName, Exception exception);

        void DebugException(string logName, Exception exception);

        void InfoException(string logName, Exception exception);

        void ErrorException(string logName, Exception exception);

        void FatalException(string logName, Exception exception);
    }
}
