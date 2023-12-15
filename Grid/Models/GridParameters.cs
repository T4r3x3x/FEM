using System.ComponentModel.DataAnnotations;

using MathModels;

namespace Grid.Models
{
	public class GridParameters : IValidatableObject
	{
		public Point[][] linesNodes;
		public int[][] areas;
		public double[] qx;
		public double[] qy;
		public List<int> xSplitsCount;
		public List<int> ySplitsCount;
		public int[][] boundaryConditions;

		public GridParameters(Point[][] linesNodes, int[][] areas, double[] qx, double[] qy, List<int> xSplitsCount, List<int> ySplitsCount, int[][] boundaryConditions)
		{
			this.linesNodes = linesNodes;
			this.areas = areas;
			this.qx = qx;
			this.qy = qy;
			this.xSplitsCount = xSplitsCount;
			this.ySplitsCount = ySplitsCount;
			this.boundaryConditions = boundaryConditions;

			var results = new List<ValidationResult>();
			var context = new ValidationContext(this);
			if (!Validator.TryValidateObject(this, context, results, true))
			{
				//foreach (var error in results)
				//{
				//	Console.WriteLine(error.ErrorMessage);
				//}
				//	throw new ValidationException(validationResult: results[0]);
				//Environment.FailFast("Application failed.");
				//	Console.ReadLine();
			}

		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> errors = new List<ValidationResult>();

			if (linesNodes[0].Length != qx.Length)
				errors.Add(new ValidationResult("Количество коэффициентов разрядки и интервалов разбиений не совпадает!"));

			//if (string.IsNullOrWhiteSpace(Name))
			//	errors.Add(new ValidationResult("Не указано имя"));

			//if (Name.Length < 2 || Name.Length > 20)
			//	errors.Add(new ValidationResult("Некорректная длина имени"));

			//if (this.Age < 1 || this.Age > 100)
			//	errors.Add(new ValidationResult("Недопустимый возраст"));

			return errors;
		}
	}
}
