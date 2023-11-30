namespace ReaserchPaper
{
	internal class Result
	{
		public IEnumerable<Vector> NumericalSolves { get; set; }
		public IEnumerable<Vector>

		static double GetSolveDifference(Vector u, Vector uk)
		{
			Vector temp = new Vector(u.Length);

			for (int i = 0; i < u.Length; i++)
				temp.Elements[i] = u.Elements[i] - uk.Elements[i];

			return temp.GetNorm() / u.GetNorm();
		}
	}
}
