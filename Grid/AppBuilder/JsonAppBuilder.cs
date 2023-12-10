using FemProducer.Logger;
using FemProducer.Models;

using Newtonsoft.Json.Linq;

namespace FemProducer.AppBuilder
{
	internal class JsonAppBuilder : IAppBuilder
	{
		private readonly string _filePath;
		private readonly ILogger _logger;

		public JsonAppBuilder(string filePath, ILogger logger)
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

		GridParameters IAppBuilder.GetGridParameters() => DeserializeJsonObject<GridParameters>(_filePath);

		ProblemService IAppBuilder.GetProblemParameters() => new ProblemService();
		SolverParameters IAppBuilder.GetSolverParameters() => DeserializeJsonObject<SolverParameters>(_filePath);
	}
}