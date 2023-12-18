using System.ComponentModel.DataAnnotations;

using MathModels;

using Tools;

namespace Grid.Models
{
	public class GridParameters : IValidatableObject
	{
		public Point[][] linesNodes;
		public int[][] areas;
		public double[] qx, qy;
		public double qt;
		public List<int> xSplitsCount, ySplitsCount;
		public int tSplitsCount;
		public double[] tLimits;
		public int[][] boundaryConditions;

		public GridParameters(Point[][] linesNodes, int[][] areas, double[] qx, double[] qy, double qt, List<int> xSplitsCount, List<int> ySplitsCount, int tSplitsCount, double[] tLimits, int[][] boundaryConditions)
		{
			this.linesNodes = linesNodes;
			this.areas = areas;
			this.qx = qx;
			this.qy = qy;
			this.qt = qt;
			this.xSplitsCount = xSplitsCount;
			this.ySplitsCount = ySplitsCount;
			this.tSplitsCount = tSplitsCount;
			this.tLimits = tLimits;
			this.boundaryConditions = boundaryConditions;

			var results = new List<ValidationResult>();
			var context = new ValidationContext(this);

			if (!Validator.TryValidateObject(this, context, results, true))
				throw new ValidationException(results.ToCommonLine());
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> errors = new List<ValidationResult>();

			if (linesNodes[0].Length != qx.Length + 1)
				errors.Add(new ValidationResult("Количество коэффициентов разрядки qx и количество интервалов разбиений не совпадает!"));

			if (linesNodes.Length != qy.Length + 1)
				errors.Add(new ValidationResult("Количество коэффициентов разрядки qy и количество интервалов разбиений не совпадает!"));


			return errors;
		}
	}
}
