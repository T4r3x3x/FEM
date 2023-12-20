using System.ComponentModel.DataAnnotations;

using Grid.Enum;

using MathModels;

using Tools;

namespace Grid.Models
{
	public class GridParameters : IValidatableObject
	{
		public Point[][] LinesNodes;
		public int[][] Areas;
		public double[] Qx, Qy, Qz;
		public double Qt;
		public List<int> XSplitsCount, YSplitsCount, ZSplitsCount;
		public double[] ZW;
		public int TSplitsCount;
		public double[] TLimits;
		public int[][] BoundaryConditions;
		public GridDimensional GridDimensional;

		public GridParameters(Point[][] linesNodes, int[][] areas, double[] qx, double[] qy, double[] qz, double qt, List<int> xSplitsCount, List<int> ySplitsCount, List<int> zSplitsCount, int tSplitsCount, double[] tLimits,
			int[][] boundaryConditions, GridDimensional gridDimensional, double[] zw)
		{
			LinesNodes = linesNodes;
			Areas = areas;
			Qx = qx;
			Qy = qy;
			Qz = qz;
			Qt = qt;
			XSplitsCount = xSplitsCount;
			YSplitsCount = ySplitsCount;
			ZSplitsCount = zSplitsCount;
			TSplitsCount = tSplitsCount;
			TLimits = tLimits;
			BoundaryConditions = boundaryConditions;
			GridDimensional = gridDimensional;
			ZW = zw;
			var results = new List<ValidationResult>();
			var context = new ValidationContext(this);

			if (!Validator.TryValidateObject(this, context, results, true))
				throw new ValidationException(results.ToCommonLine());
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> errors = new List<ValidationResult>();

			if (LinesNodes[0].Length != Qx.Length + 1)
				errors.Add(new ValidationResult("Количество коэффициентов разрядки qx и количество интервалов разбиений не совпадает!"));

			if (LinesNodes.Length != Qy.Length + 1)
				errors.Add(new ValidationResult("Количество коэффициентов разрядки qy и количество интервалов разбиений не совпадает!"));


			return errors;
		}
	}
}
