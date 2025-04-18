// Utils/LogHelper.cs
using log4net;
using log4net.Config;
using System;
using System.IO;

namespace ProgressCrystalReport.Utils
{
    /// <summary>
    /// Classe di utilità per il logging con log4net
    /// </summary>
    public static class LogHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LogHelper));
        private static bool _isInitialized = false;

        /// <summary>
        /// Inizializza il sistema di logging
        /// </summary>
        public static void Initialize()
        {
            if (!_isInitialized)
            {
                XmlConfigurator.Configure(new FileInfo("log4net.config"));
                _isInitialized = true;
            }
        }

        public static void LogInfo(string message)
        {
            Log.Info(message);
        }

        public static void LogDebug(string message)
        {
            Log.Debug(message);
        }

        public static void LogWarning(string message)
        {
            Log.Warn(message);
        }

        public static void LogError(string message, Exception ex = null)
        {
            if (ex == null)
            {
                Log.Error(message);
            }
            else
            {
                Log.Error(message, ex);
            }
        }
    }
}