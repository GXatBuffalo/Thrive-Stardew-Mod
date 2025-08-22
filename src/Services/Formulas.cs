using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thrive.src.Services
{
	public static class Formulas
	{
		private static double Hash01(int x, int y)
		{
			int n = x * 73856093 ^ y * 19349663;
			n = n << 13 ^ n;
			uint nn = (uint)(n * (n * n * 15731 + 789221) + 1376312589);
			return (nn & 0xFFFFFF) / (double)0x1000000;
		}

		public static double HighClusterSqrt(int x, int y, Random rand)
				=> 50 + 50 * Math.Sqrt(Hash01(x, y));

		public static double HighClusterPow(int x, int y, Random rand)
				=> 50 + 50 * Math.Pow(Hash01(x, y), 0.5);

		public static double StableWithJitter(int x, int y, Random rand)
		{
			double core = 50 + 50 * Math.Sqrt(Hash01(x, y));
			return Math.Clamp(core + (rand.NextDouble() * 6 - 3), 0, 100);
		}

		public static double Logistic(int x, int y, Random rand)
		{
			double t = (Hash01(x, y) - 0.5) * 10; // spread ~ [-5,5]
			double sig = 1.0 / (1.0 + Math.Exp(-t));
			double jitter = rand.NextDouble() * 0.06 - 0.03;
			return 100 * Math.Clamp(sig + jitter, 0, 1);
		}

		// Collect them all here
		public static readonly List<Formulas> All = new()
		{
				HighClusterSqrt,
				HighClusterPow,
				StableWithJitter,
				Logistic
        // Add more as you invent them
    };
	}
}
