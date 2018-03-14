#if UNITY_5_3_OR_NEWER
using UnityEngine;

namespace CryoDI
{
	internal static class DILog
	{
		public static void Log(string message)
		{
			Debug.Log(message);
		}

		public static void LogWarning(string message)
		{
			Debug.LogWarning(message);
		}

		public static void LogError(string message)
		{
			Debug.LogError(message);
		}
	}
}
#else

using log4net;

namespace CryoDI
{
	internal static class DILog
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(DILog));

		public static void Log(string message)
		{
			_log.Info(message);
		}

		public static void LogWarning(string message)
		{
			_log.Warn(message);
		}

		public static void LogError(string message)
		{
			_log.Error(message);
		}
	}
}
#endif