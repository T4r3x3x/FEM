using System.Diagnostics;

namespace Tools
{
    public static class Processes
    {
        /// <summary>
        /// Запускает python скрипт.
        /// </summary>
        /// <param name="scriptPath">Путь, в котором находится скрипт</param>
        /// <param name="args">аргументы для скрипта</param>
        public static void OpenPythonScript(string scriptPath, params string[] args)
        {
            ProcessStartInfo start = new()
            {
                FileName = "python",
                Arguments = $"{scriptPath} {string.Join(' ', args)}",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            Process.Start(start);
        }
    }
}
