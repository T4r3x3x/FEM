using ResearchPaper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReaserchPaper.Solver
{
	internal interface ISolver
	{
		public Vector Solve(Matrix A, Vector f);
	}
}
