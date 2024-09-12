namespace Dupery
{
	class Logger
	{
		public static void Log(string message)
		{
			Debug.Log($"[Dupery] {message}");
		}

		public static void LogError(string message)
		{
			Debug.LogError($"[Dupery] {message}");
		}
	}
}
