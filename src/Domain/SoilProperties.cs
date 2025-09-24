

namespace Thrive.src.Domain
{
	public class SoilProperties
	{
		public List<double> SoilStats { get; set; }
		public List<int> Health { get; set; } = new List<int> { 100, 100, 100, 100, 100 };
		public GrowingCropStats? CropHere { get; set; }
		public string? CropID { get; set; }

		public SoilProperties(Random rand, int propertyCount, int x, int y, List<Formulas.SoilInitializationFormulas> appliedFormula)
		{
			InitializeSoil(rand, propertyCount, x, y, appliedFormula);
		}

		// start new soil stats 
		public void InitializeSoil(Random rand, int propertyCount, int x, int y, List<Formulas.SoilInitializationFormulas> appliedFormula)
		{
			// for index 0 to propertyCount, each respective element(soil property) is set to the result of their respective soil initialization formula
			for (int i = 0; i < propertyCount; i++)
			{
				SoilStats[i] = appliedFormula[i](rand, x, y, i);
			}
		}

		// assign a crop to this tile
		public void AddCrop(string cid, BaseCropData cData, int soilPropertyCount){
			CropHere = new GrowingCropStats(cData.StarterHealthStats, soilPropertyCount);
			CropID = cid;
		}

		// unassign the crop stored on this tile
		public void DeleteCrop(){
			CropHere = null;
			CropID = null;
		}

		// when crop is destroyed or similar, return part of its health to soil, default 10%
		// temporary
		public void ReturnCropToSoil(int conversionRate = 10){
			for (int i = 0; i < CropHere.HealthStats.Count; i++){
				SoilStats[i] += CropHere.HealthStats[i] / conversionRate;
			}
			CropHere = null;
			CropID = null;
		}

		// STUB
		// note: remember to act accordingly if no crop exists on this tile
		public void UpdateSoilandCropHealth(Dictionary<string, Domain.BaseCropData> CropDict)
		{
			Domain.BaseCropData cd = CropDict[CropID];

		}
	}
}