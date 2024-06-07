using System.ComponentModel.DataAnnotations;

using Grid.Enum;

using MathModels;

using Tools;

namespace Grid.Models.InputModels
{
    public class GridInputParameters : IValidatableObject
    {
        public Point[][] LinesNodes;
        public Area<int>[] Areas;
        public AxisInputParameters XParams;
        public AxisInputParameters YParams;
        public AxisInputParameters ZParams;
        public AxisInputParameters TParams;
        public double[] ZW;
        public double[] TW;
        public (Area<int> Area, BoundaryType BoundaryType)[] BoundaryConditions;
        public GridDimensional GridDimensional;

        public GridInputParameters(Point[][] linesNodes, int[][] areas, AxisInputParameters xParams, AxisInputParameters yParams,
            AxisInputParameters zParams, AxisInputParameters tParams, double[] zW, double[] tW, int[][] boundaryConditions,
            GridDimensional gridDimensional)
        {
            LinesNodes = linesNodes;
            Areas = new Area<int>[areas.Length];
            for (int i = 0; i < areas.Length; i++)
            {
                Areas[i] = new Area<int>(areas[i], areas[i][6]);
            }
            XParams = xParams;
            YParams = yParams;
            ZParams = zParams;
            TParams = tParams;
            ZW = zW;
            TW = tW;
            BoundaryConditions = new (Area<int> Area, BoundaryType)[boundaryConditions.Length];
            for (int i = 0; i < boundaryConditions.Length; i++)
            {
                BoundaryConditions[i].Area = new Area<int>(boundaryConditions[i], boundaryConditions[i][7]);
                BoundaryConditions[i].BoundaryType = (BoundaryType) boundaryConditions[i][6];
            }
            GridDimensional = gridDimensional;

            var results = new List<ValidationResult>();
            var context = new ValidationContext(this);

            if (!Validator.TryValidateObject(this, context, results, true))
                throw new ValidationException(results.ToCommonLine());
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errors = new List<ValidationResult>();

            //if (LinesNodes[0].Length != Qx.Length + 1)
            //    errors.Add(new ValidationResult("Количество коэффициентов разрядки qx и количество интервалов разбиений не совпадает!"));

            //if (LinesNodes.Length != Qy.Length + 1)
            //    errors.Add(new ValidationResult("Количество коэффициентов разрядки qy и количество интервалов разбиений не совпадает!"));


            return errors;
        }
    }
}
