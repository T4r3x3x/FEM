namespace FemProducer.Basises.BasisFunctions
{
	public static class LinearQuarangularBasisFunctions
	{
		public static List<Func<double, double, double>> fitasKsi = [
			(ksi, nu) => nu - 1,
			(ksi, nu) => 1 - nu,
			(ksi, nu) => -nu,
			(ksi, nu) => nu,
		];

		public static List<Func<double, double, double>> fitasNu = [
			(ksi, nu) => ksi - 1,
			(ksi, nu) => -ksi,
			(ksi, nu) => 1 - ksi,
			(ksi, nu) => ksi,
		];

		public static List<Func<double, double>> W = [
			(a) => 1 - a,
			(a) => a,
		];

		public static List<Func<double, double, double>> Fita = [
			(ksi, nu) => W[0](ksi) * W[0](nu),
			(ksi, nu) => W[1](ksi) * W[0](nu),
			(ksi, nu) => W[0](ksi) * W[1](nu),
			(ksi, nu) => W[1](ksi) * W[1](nu),
		];
	}
}
