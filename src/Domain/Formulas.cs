using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thrive.src.Domain
{
	public static class Formulas
	{
		public delegate double SoilInitializationFormulas(Random rand, int x, int y, int index);
		public static readonly List<SoilInitializationFormulas> SoilInitFormulas = new()
		{

		};

		public delegate double CropRequirementFormula(Random rand, int a, int b, int c, int d);
		public static readonly List<CropRequirementFormula> CropReqFormulas = new()
		{
				formulaB,
				HighClusterSqrt,
				HighClusterPow,
				StableWithJitter,
				Logistic
		};

		public delegate double CropDepreciationFormula(Random rand, int a, int b, int c, int d);
		public static readonly List<CropDepreciationFormula> CropDepreFormulas = new()
		{

		};

		public static double ApplyConfigsAndLevel(){
			return 0.0;
		}

		public static double numberMerge(Random rand, int a, int b, int c, int d)
		{
			double crop_factor = (Math.Sqrt(a * (b * 3.625) * c * 3) / d);
			return crop_factor;
		}

		public static double formulaB(Random rand, int a, int b, int c, int d)
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

		public static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);

	}
}
