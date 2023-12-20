using FemProducer.Models;

using Grid.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using SlaeSolver.Models;

namespace FemProducer.ConfigureReader
{
	public class JsonConfigureReader : IConfigureReader
	{
		private readonly string _filePath;

		public JsonConfigureReader(string filePath)
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

		GridParameters IConfigureReader.GetGridParameters() => DeserializeJsonObject<GridParameters>(_filePath);
		ProblemParameters IConfigureReader.GetProblemParameters() => DeserializeJsonObject<ProblemParameters>(_filePath);
		SolverParameters IConfigureReader.GetSolverParameters() => DeserializeJsonObject<SolverParameters>(_filePath);
	}
}