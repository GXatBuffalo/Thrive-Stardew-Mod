using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;

namespace Thrive.src.Domain
{
	public class SoilNutrition
	{
		public string CropID { get; set; }
		public List<double> SoilStats { get; set; }
		public List<int> Health { get; set; } = new List<int> { 100, 100, 100, 100, 100 };

		private SoilNutrition(string id, Random rand, int nutriCount, int x, int y, Formulas.InitializationFormulas appliedFormula)
		{
			CropID = id;
			InitializeSoil(rand, nutriCount, x, y, appliedFormula);
		}
		
		private void InitializeSoil(Random rand, int nutriCount, int x, int y, Formulas.InitializationFormulas appliedFormula)
		{
			for (int i = 0; i < nutriCount; i++)
			{
				SoilStats[i] = appliedFormula(rand, x, y, i);
			}
		}

}