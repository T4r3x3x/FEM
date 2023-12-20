using System.ComponentModel.DataAnnotations;

using Tools;

namespace FemProducer.Models
{
	public class ProblemParameters : IValidatableObject
	{
		public IList<double> Lambda, Gamma;

		public ProblemParameters(IList<double> lambda, IList<double> gamma)
		{
			Lambda = lambda;
			Gamma = gamma;

			var results = new List<ValidationResult>();
			var context = new ValidationContext(this);

			if (!Validator.TryValidateObject(this, context, results, true))
				throw new ValidationException(results.ToCommonLine());
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			List<ValidationResult> errors = new List<ValidationResult>();
			if (Lambda.Count == 0)
				errors.Add(new ValidationResult("Не задано ни одной лямбды!"));

			if (Gamma.Count == 0)
				errors.Add(new ValidationResult("Не задано ни одной гаммы!"));

			return errors;
		}
	}
}
