using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thrive.src.Domain
{
	internal class Formulas
	{
		public delegate double Formula(Random rand, int a, int b, int c, int d);

		private static double numberMerge(Random rand, int a, int b, int c, int d)
		{
			double crop_factor = (Math.Sqrt(a * (b * 3.625) * c * 3) / d);
			return crop_factor;
		}

		private static double formulaB(Random rand, int a, int b, int c, int d)
		{
			return (Math.Sqrt(200 / numberMerge(rand, a, b, c, d)) - 3.0) * 1.2;
		}

		public static double HighClusterSqrt(Random rand, int a, int b, int c, int d)
				=> 40 + 40 * Math.Sqrt(a*b);

		public static double HighClusterPow(Random rand, int a, int b, int c, int d)
				=> 40 + 40 * Math.Pow(a/b, 2.0);

		public static double StableWithJitter(Random rand, int a, int b, int c, int d)
		{
			double core = 50 + 50 * Math.Sqrt(numberMerge(rand, a, b, c, d));
			return Math.Clamp(core + (rand.NextDouble() * 6 - 3), 0, 100);
		}

		public static double Logistic(Random rand, int a, int b, int c, int d)
		{
			double t = (numberMerge(rand, a,b,c,d) - 0.5) * 10; // spread ~ [-5,5]
			double sig = 1.0 / (1.0 + Math.Exp(-t));
			double jitter = rand.NextDouble() * 0.06 - 0.03;
			return 100 * Math.Clamp(sig + jitter, 0, 1);
		}
		public static readonly List<Formula> All = new()
		{
				formulaB,
				HighClusterSqrt,
				HighClusterPow,
				StableWithJitter,
				Logistic
		};

	}
}
