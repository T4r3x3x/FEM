using ReaserchPaper.Logger;

namespace FemProducer.Logger
{
	internal class TxtLogger : ILogger
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
