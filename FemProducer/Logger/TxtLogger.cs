namespace FemProducer.Logger
{
	public class TxtLogger : ILogger
	{
		private readonly string _path;

		public TxtLogger(string path)
		{
			_path = path;
		}

		public void Log(string message)
		{
			using StreamWriter stream = new StreamWriter(_path);

		}
	}
}
