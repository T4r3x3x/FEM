namespace Tools
{
	public static class Messages
	{
		public static void PrintErrorMessage(string message)
		{
			Console.WriteLine(message, Console.BackgroundColor = ConsoleColor.DarkRed);
			Console.BackgroundColor = ConsoleColor.Gray;
		}
		public static void PrintSuccessMessage(string message)
		{
			Console.WriteLine(message, Console.BackgroundColor = ConsoleColor.Green);
			Console.BackgroundColor = ConsoleColor.Gray;
		}
	}
}
