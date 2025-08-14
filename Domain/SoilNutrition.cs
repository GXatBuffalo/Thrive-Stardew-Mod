using System;
using System.Collections.Generic;

namespace StardewNutrition.Domain
{
	public class SoilNutrition
	{
		public string CropID { get; set; }
		public List<int> SoilStats { get; set; }
		public List<int> Health { get; set; } = new List<int> { 100, 100, 100, 100, 100 };

		public SoilNutrition(string id, int nitro, int phos, int ph, int iridium)
		{
			CropID = id;
			SoilStats = new List<int> { nitro, phos, ph, iridium };
		}
	}
}
