using FemProducer;
using FemProducer.DTO;

using Newtonsoft.Json.Linq;

using ReaserchPaper.Logger;
using ReaserchPaper.Solver;

using ResearchPaper;

namespace ReaserchPaper
{
	internal class JsonTaskBuilder : ITaskBuilder
	{
		private readonly string _filePath;
		private readonly ILogger _logger;

		public JsonTaskBuilder(string filePath, ILogger logger)
		{
			_filePath = filePath;
			_logger = logger;
		}

		private ObjectType DeserializeJsonObject<ObjectType>(string filePath)
		{
			using StreamReader streamReader = new(filePath);

			var jObject = JObject.Parse(streamReader.ReadToEnd());
			var jToken = jObject.GetValue(typeof(ObjectType).Name);
			ObjectType result = jToken.ToObject<ObjectType>();
			if (result == null)
				throw new Exception("Cound't read grid paramters");
			return result;
		}

		GridParameters ITaskBuilder.GetGridParameters() => DeserializeJsonObject<GridParameters>(this._filePath);

		ProblemParametrs ITaskBuilder.GetProblem() => new ProblemParametrs();
		ISolver ITaskBuilder.GetSolver() => new LosLU();

		public class Item
		{
			public int millis;
			public string stamp;
			public DateTime datetime;
			public string light;
			public float temp;
			public float vcc;
		}
	}
}
