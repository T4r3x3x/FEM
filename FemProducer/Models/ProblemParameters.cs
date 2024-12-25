using System.ComponentModel.DataAnnotations;

using Tools;

namespace FemProducer.Models;

public class ProblemParameters : IValidatableObject
{
    public IList<double> SourcePowers { get; }
    public IList<double> Lambda { get; }
    public IList<double> Gamma { get; }

    public ProblemParameters(IList<double> lambda, IList<double> gamma, IList<double> sourcePowers)
    {
        Lambda = lambda;
        Gamma = gamma;
        SourcePowers = sourcePowers;

        var results = new List<ValidationResult>();
        var context = new ValidationContext(this);

        if (!Validator.TryValidateObject(this, context, results, true))
            throw new ValidationException(results.ToCommonLine());
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<ValidationResult>();
        if (Lambda.Count == 0)
            errors.Add(new("Не задано ни одной лямбды!"));

        if (Gamma.Count == 0)
            errors.Add(new("Не задано ни одной гаммы!"));

        return errors;
    }
}