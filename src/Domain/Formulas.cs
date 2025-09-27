
using StardewValley.Tools;

namespace Thrive.src.Domain
{
	public static class Formulas
	{
		public delegate double SoilInitializationFormulas(Random rand, int x, int y, int index);
		public static readonly List<SoilInitializationFormulas> SoilInitFormulas = new()
		{
			formulaA3,
			formulaB3,
			formulaC3,
			formulaD3,
			formulaE3,
			formulaF3
		};

		public delegate double CropRequirementFormula(Random rand, int a, int b, int c, int d);
		public static readonly List<CropRequirementFormula> CropReqFormulas = new()
		{
			formulaA,
			formulaB,
			formulaC,
			formulaD,
			formulaE,
			formulaF
		};

		public delegate double CropDepreciationFormula(Random rand, int a, int b, int c, int d);
		public static readonly List<CropDepreciationFormula> CropDepreFormulas = new()
		{
			formulaA2,
			formulaB2,
			formulaC2,
			formulaD2,
			formulaE2,
			formulaF2
		};

		public static double ApplyConfigsAndLevel(){
			return 0.0;
		}

		public static double numberMerge(Random rand, int a, int b, int c, int d)
		{
			double crop_factor = (Math.Sqrt(a * (b * 3.625) * c * 3) / d);
			return crop_factor;
		}

		public static double formulaA(Random rand, int a, int b, int c, int d)
		{
			return (Math.Sqrt(200 / numberMerge(rand, a, b, c, d)) - 3.0) * 1.2;
		}

		public static double formulaB(Random rand, int a, int b, int c, int d)
				=> 40 + 40 * Math.Sqrt(a*b);

		public static double formulaC(Random rand, int a, int b, int c, int d)
				=> 40 + 40 * Math.Pow(a/b, 2.0);

		public static double formulaD(Random rand, int a, int b, int c, int d)
		{
			double core = 50 + 50 * Math.Sqrt(numberMerge(rand, a, b, c, d));
			return Math.Clamp(core + (rand.NextDouble() * 6 - 3), 0, 100);
		}

		public static double formulaE(Random rand, int a, int b, int c, int d)
		{
			double t = (numberMerge(rand, a,b,c,d) - 0.5) * 10; // spread ~ [-5,5]
			double sig = 1.0 / (1.0 + Math.Exp(-t));
			double jitter = rand.NextDouble() * 0.06 - 0.03;
			return 100 * Math.Clamp(sig + jitter, 0, 1);
		}

		public static double formulaF(Random rand, int a, int b, int c, int d)
				=> Math.Clamp(Math.Sin(numberMerge(rand, a, b, c, d)) * 100, 0, 100);

		public static double formulaA2(Random rand, int a, int b, int c, int d)
		{
			return formulaA(rand, a, b, c, d) / 40;
		}

		public static double formulaB2(Random rand, int a, int b, int c, int d)
				=> formulaB(rand, a, b, c, d) / 40;

		public static double formulaC2(Random rand, int a, int b, int c, int d)
				=> formulaB(rand, a, b, c, d) / 30;

		public static double formulaD2(Random rand, int a, int b, int c, int d)
		{
			return formulaD(rand, a, b, c, d) / 40; ;
		}

		public static double formulaE2(Random rand, int a, int b, int c, int d)
		{
			return formulaE(rand, a, b, c, d) / 50; ;
		}

		public static double formulaF2(Random rand, int a, int b, int c, int d)
				=> Math.Sin(numberMerge(rand, a, b, c, d)) * 1.3;

		public static double formulaA3(Random rand, int x, int y, int index){
			return 50.0;
		}

		public static double formulaB3(Random rand, int x, int y, int index)
		{
			return 60.0;
		}

		public static double formulaC3(Random rand, int x, int y, int index)
		{
			return 70.0;
		}

		public static double formulaD3(Random rand, int x, int y, int index)
		{
			return rand.Next(50,70);
		}

		public static double formulaE3(Random rand, int x, int y, int index)
		{
			return rand.Next(40, 80);
		}

		public static double formulaF3(Random rand, int x, int y, int index)
		{
			return rand.Next(30, 90);
		}

		public static int Clamp(int value, int min, int max) => Math.Min(Math.Max(value, min), max);

	}
}
