using System.Diagnostics;
using GameFramework;
using Debug = UnityEngine.Debug;

namespace Game
{
    public static class Log
    {
        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_DEBUG_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        public static void Info(object info)
        {
            if (!Framework.FrameworkActiviting)
            {
                Debug.Log(info);
            }
            else
            {
                GameFrameworkLog.Info(info);
            }
        }

        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_DEBUG_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        public static void Info(string info)
        {
            if (!Framework.FrameworkActiviting)
            {
                Debug.Log(info);
            }
            else
            {
                GameFrameworkLog.Info(info);
            }
        }

        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_ERROR_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
        [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
        [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
        public static void Error(string error)
        {
            if (!Framework.FrameworkActiviting)
            {
                Debug.LogError(error);
            }
            else
            {
                GameFrameworkLog.Error(error);
            }
        }

        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_ERROR_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
        [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
        [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
        public static void LogAdvertisement(string info)
        {
            var s = $"ADVERTISEMENT: {info}";
            if (!Framework.FrameworkActiviting)
            {
                Debug.Log(s);
            }
            else
            {
                GameFrameworkLog.Info(s);
            }
        }

        [Conditional("ENABLE_LOG")]
        [Conditional("ENABLE_ERROR_LOG")]
        [Conditional("ENABLE_DEBUG_AND_ABOVE_LOG")]
        [Conditional("ENABLE_INFO_AND_ABOVE_LOG")]
        [Conditional("ENABLE_WARNING_AND_ABOVE_LOG")]
        [Conditional("ENABLE_ERROR_AND_ABOVE_LOG")]
        public static void LogEventReport(string eventName, string info)
        {
            var s = $"打点{eventName}: {info}";
            if (!Framework.FrameworkActiviting)
            {
                Debug.Log(s);
            }
            else
            {
                GameFrameworkLog.Info(s);
            }
        }
    }
}