using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Thrive.src.Domain
{
	public class SoilProperties
	{
		public string CropID { get; set; }
		public List<double> SoilStats { get; set; }
		public List<int> Health { get; set; } = new List<int> { 100, 100, 100, 100, 100 };

		public SoilProperties(string id, Random rand, int propertyCount, int x, int y, Formulas.SoilInitializationFormulas appliedFormula)
		{
			CropID = id;
			InitializeSoil(rand, propertyCount, x, y, appliedFormula);
		}

		public void InitializeSoil(Random rand, int propertyCount, int x, int y, Formulas.SoilInitializationFormulas appliedFormula)
		{
			for (int i = 0; i < propertyCount; i++)
			{
				SoilStats[i] = appliedFormula(rand, x, y, i);
			}
		}
	}
}