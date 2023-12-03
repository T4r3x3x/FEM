using FemProducer;
using FemProducer.DTO;

using ReaserchPaper.Solver;

namespace ReaserchPaper
{
	internal class JsonTaskBuilder : ITaskBuilder
	{
		GridParametrs ITaskBuilder.GetGridParametrs() => throw new NotImplementedException();
		ProblemParametrs ITaskBuilder.GetProblem() => throw new NotImplementedException();
		ISolver ITaskBuilder.GetSolver() => throw new NotImplementedException();

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
