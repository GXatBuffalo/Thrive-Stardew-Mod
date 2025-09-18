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
		public GrowingCropStats CropHere { get; set; }

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

		public void AddCrop(BaseCropData cData, int soilPropertyCount){
			CropHere = new GrowingCropStats(CropID, cData.StarterHealthStats, soilPropertyCount);
		}

		public void DeleteCrop(){
			CropHere = null;
		}

		public void ReturnCropToSoil(int conversionRate = 10){
			for (int i = 0; i < CropHere.HealthStats.Count; i++){
				SoilStats[i] += CropHere.HealthStats[i] / conversionRate;
			}
			CropHere = null;
		}
	}
}