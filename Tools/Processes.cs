using System.Diagnostics;

namespace Tools
{
	public static class Processes
	{
		/// <summary>
		/// Запускает python скрипт.
		/// </summary>
		/// <param name="scriptPath">Путь, в котором находится скрипт</param>
		public static void OpenPythonScript(string scriptPath)
		{
			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = "python";
			start.Arguments = string.Format(scriptPath);
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			Process.Start(start);
		}
	}
}
