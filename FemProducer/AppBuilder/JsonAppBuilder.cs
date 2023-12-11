using Grid.Models;

using Newtonsoft.Json.Linq;

using SlaeSolver.Models;

namespace FemProducer.AppBuilder
{
	internal class JsonAppBuilder : IAppBuilder
	{
		private readonly string _filePath;

		public JsonAppBuilder(string filePath)
		{
			_filePath = filePath;
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