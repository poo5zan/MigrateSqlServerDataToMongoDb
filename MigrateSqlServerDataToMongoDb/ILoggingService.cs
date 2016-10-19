using System;

namespace MigrateSqlServerDataToMongoDb
{
    public interface ILoggingService
    {
        /// <summary>
        /// Writes Log to file
        /// </summary>
        /// <param name="message">Message to Log</param>
        /// <param name="exception">Exception</param>
        /// <param name="logLevel">LogLevel 
        /// </param>
        /// <param name="type">Type of Logger , simply Class Name of calling code</param>
        void Write(object message,
            LogLevel logLevel = LogLevel.None,
            Type type = null,
            bool writeAllObjectProperties = false,
            Exception exception = null);
    }
}
