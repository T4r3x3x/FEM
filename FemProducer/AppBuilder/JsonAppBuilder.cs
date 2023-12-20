using FemProducer.Models;

using Grid.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SlaeSolver.Models;

namespace FemProducer.AppBuilder
{
	public class JsonAppBuilder : IAppBuilder
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
				throw new JsonReaderException("Can't read object " + typeof(ObjectType).Name);

			ObjectType result = jToken.ToObject<ObjectType>();
			return result;

		}

		GridParameters IAppBuilder.GetGridParameters() => DeserializeJsonObject<GridParameters>(_filePath);
		ProblemParameters IAppBuilder.GetProblemParameters() => DeserializeJsonObject<ProblemParameters>(_filePath);
		SolverParameters IAppBuilder.GetSolverParameters() => DeserializeJsonObject<SolverParameters>(_filePath);
	}
}