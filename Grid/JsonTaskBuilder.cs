using FemProducer;
using FemProducer.DTO;

using Newtonsoft.Json.Linq;

using ReaserchPaper.Logger;

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
			if (jToken == null)
				throw new Exception("Cound't read object " + typeof(ObjectType).Name);
			ObjectType result = jToken.ToObject<ObjectType>();
			return result;
		}

		GridParameters ITaskBuilder.GetGridParameters() => DeserializeJsonObject<GridParameters>(this._filePath);

		ProblemParametrs ITaskBuilder.GetProblemParameters() => new ProblemParametrs();
		SolverParameters ITaskBuilder.GetSolverParameters() => DeserializeJsonObject<SolverParameters>(this._filePath);
	}
}