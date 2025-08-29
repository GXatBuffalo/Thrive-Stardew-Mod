using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using Thrive.src;
using Thrive.src.Domain;

namespace Thrive.src.Services
{
	public class FarmingHandler
	{
		public IMonitor Monitor { get; }
		public IModHelper gameHandler { get; }

		public List<string> SoilNutrientNames { get; set; } = new List<string> { "Nitro", "Phos", "pH", "Aera", "Microbes" };
		public int nutriMin = 0;
		public int nutriMax = 1000;
		public NutritionMap CurrentMap { get; private set; } = new NutritionMap(0, 0, 0);
		public string? CurrentKey { get; private set; } 
		public Dictionary<string, CropData> KnownCropDict { get; set; } = new Dictionary<string, CropData>();
		private List<Formulas.Formula> FormulaList { get; set; }

		public FarmingHandler(IModHelper helper, IMonitor monitor)
		{
			Monitor = monitor;
			gameHandler = helper;
			Random rand = new Random((int)Game1.uniqueIDForThisGame);
			FormulaList = Formulas.CropFormulas.OrderBy(_ => rand.Next()).Take(5).ToList();
		}

		public void InitializeFormulas(){
			List<Formulas.Formula> TempFormulaList = gameHandler.Data.ReadSaveData<List<Formulas.Formula>>("Thrive.FormulaList");
			Random rand = new Random((int)Game1.uniqueIDForThisGame);
			FormulaList = Formulas.CropFormulas.OrderBy(_ => rand.Next()).Take(5).ToList();
		}

		public void StarterMap()
		{
			int width = Game1.currentLocation.Map.Layers[0].LayerWidth;
			int height = Game1.currentLocation.Map.Layers[0].LayerHeight;
			Random rand = new Random();
			// 10-30 is beginning mana, remember to rebalance
			CurrentMap = new NutritionMap(width, height, rand.Next(10, 30));
			CurrentKey = Game1.currentLocation.Name;
		}

		public void LoadCurrentMap()
		{
			CurrentKey = Game1.currentLocation.Name;
			CurrentMap = gameHandler.Data.ReadSaveData<NutritionMap>(CurrentKey);
		}

		public void SaveCurrentMap()
		{
			gameHandler.Data.WriteSaveData(CurrentKey, CurrentMap);
		}

		// REMINDER: Fix numbers, REMOVE MAGIC NUMBERS
		public void UpdateSoilAndCropHealth(SoilNutrition sn, CropData cd)
		{
			var configs = gameHandler.ReadConfig<ModConfig>();
			for (int x = 0; x < configs.SoilNutritionCount-2; x++)
			{
				if (Math.Abs(sn.SoilStats[x] - cd.Requirements[x * 2]) <= Math.Abs(cd.Requirements[x * 2 + 1]))
					sn.Health[x] += 10;
				else
					sn.Health[x] -= 18;

				sn.SoilStats[x] -= cd.SoilDeprecation[x];
			}

			if (cd.isMagic)
			{
				if (Math.Abs(sn.SoilStats[4] - cd.Requirements[8]) <= Math.Abs(cd.Requirements[9]))
					sn.Health[4] += 10;
				else
					sn.Health[4] -= 18;

				sn.SoilStats[4] -= cd.SoilDeprecation[4];
			}
		}

		public void TestSetAllCropData()
		{
			var seedData = Game1.cropData;
			Dictionary<string, List<int>> results = new();
			Random rand = new Random((int)Game1.uniqueIDForThisGame);
			foreach (KeyValuePair<string, StardewValley.GameData.Crops.CropData> kvp in seedData)
			{
				Game1.objectData.TryGetValue(kvp.Value.HarvestItemId, out var produceData);
				Monitor.Log(kvp.Value.HarvestItemId, LogLevel.Trace);
				try
				{
					results.Add(kvp.Key,
						new List<int>{
						produceData.Price,
						produceData.Edibility is -300 or 0 ? rand.Next(1,300) : Math.Abs(produceData.Edibility),
						(int)(16.0 * Math.Log(0.018 * produceData.Price + 1.0, Math.E)),
						kvp.Value.DaysInPhase.Sum(),
						produceData.Category
						}
					);
					KnownCropDict[kvp.Key] = new CropData(kvp.Key, rand);
				}
				catch { };
			}
			gameHandler.Data.WriteSaveData("Thrive.knowcropdict", KnownCropDict);
			gameHandler.Data.WriteSaveData("Thrive.knowcropdict_vanilla", results);
			gameHandler.Data.WriteJsonFile("exported-data.json", KnownCropDict);
			gameHandler.Data.WriteJsonFile("exported-data-v.json", results);
		}
	}
}
