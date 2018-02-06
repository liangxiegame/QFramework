using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSG.Utils
{
    /// <summary>
    /// Interface for logging errors.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log an error.
        /// </summary>
        void LogError(string message, params object[] args);

        /// <summary>
        /// Log an error.
        /// </summary>
        void LogError(Exception ex, string message, params object[] args);

        /// <summary>
        /// Log an info message.
        /// </summary>
        void LogInfo(string message, params object[] args);

        /// <summary>
        /// Log a warning message.
        /// </summary>
        void LogWarning(string message, params object[] args);

        /// <summary>
        /// Log a verbose message, by default these aren't output.
        /// </summary>
        void LogVerbose(string message, params object[] args);
    }
}
