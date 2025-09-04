using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.Crops;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using Thrive.src;
using Thrive.src.Domain;

namespace Thrive.src.Services
{
	public class FarmingHandler
	{
		public IMonitor Monitor { get; }
		public IModHelper gameHandler { get; }

		public List<string> SoilNutrientNames { get; set; } = new List<string> { "Nitro", "Phos", "Aera", "pH", "Microbes" };
		public int nutriMin = 0;
		public int nutriMax = 1000;
		public NutritionMap CurrentMap { get; private set; } = new NutritionMap(0, 0, 0);
		public string? CurrentKey { get; private set; } 
		public Dictionary<string, Domain.CropData> KnownCropDict { get; set; } = new Dictionary<string, Domain.CropData>();
		private List<Formulas.CropRequirementFormula> FormulaList { get; set; }

		public FarmingHandler(IModHelper helper, IMonitor monitor)
		{
			Monitor = monitor;
			gameHandler = helper;
			Random rand = new Random((int)Game1.uniqueIDForThisGame);
			FormulaList = Formulas.CropReqFormulas.OrderBy(_ => rand.Next()).Take(5).ToList();
		}

		//remember to run and save somewhere if formulas is null
		public void InitializeFormulas(){
			List<Formulas.CropRequirementFormula> TempFormulaList = gameHandler.Data.ReadSaveData<List<Formulas.CropRequirementFormula>>("Thrive.FormulaList");
			Random rand = new Random((int)Game1.uniqueIDForThisGame);
			TempFormulaList = Formulas.CropReqFormulas.OrderBy(_ => rand.Next()).Take(5).ToList();
		}

		public void StartMap()
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
		public void UpdateSoilAndCropHealth(SoilNutrition sn, Domain.CropData cd)
		{
			var configs = gameHandler.ReadConfig<ModConfig>();
			for (int x = 0; x < configs.SoilNutritionCount+2; x++)
			{
				if (Math.Abs(sn.SoilStats[x] - cd.Requirements[x * 2]) <= Math.Abs(cd.Requirements[x * 2 + 1]))
					sn.Health[x] += 10;
				else
					sn.Health[x] -= 18;

				sn.SoilStats[x] -= cd.SoilDeprecation[x];
			}
		}

		public static int newCropQuality(StardewValley.Object o, int x, int y){
			return 1;
		}

		public static int newForageQuality(StardewValley.Object o, int x, int y)
		{
			return 1;
		}


		/*
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
		*/
	}
}
