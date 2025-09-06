using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Extensions;
using StardewValley.GameData.Crops;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Numerics;
using Thrive.src;
using Thrive.src.Domain;

namespace Thrive.src.Services
{
	public class FarmingHandler
	{
		public IMonitor Monitor { get; }
		public IModHelper gameHandler { get; }

		public List<string> SoilNutrientNames { get; set; } = new List<string> { "Nitro", "Phos", "Aera", "pH", "Microbes" };
		public int propertyMin = 0;
		public int propertyMax = 1000;
		public SoilPropertiesMap CurrentMap { get; private set; } = new SoilPropertiesMap(0, 0, 0);
		public Dictionary<string, SoilPropertiesMap> AllMaps { get; private set; } = new();
		public string? CurrentMapKey { get; private set; }
		bool curMapSaved { get; set; } = false;
		public Dictionary<string, Domain.CropData> KnownCropDict { get; set; } = new Dictionary<string, Domain.CropData>();
		private List<Formulas.CropRequirementFormula> FormulaList { get; set; }
		

		public FarmingHandler(IModHelper helper, IMonitor monitor)
		{
			Monitor = monitor;
			gameHandler = helper;
			Random rand = new Random((int)Game1.uniqueIDForThisGame);
			FormulaList = InitializeFormulas();
		}

		public List<Formulas.CropRequirementFormula> InitializeFormulas(){
			int soilPropertiesCount = gameHandler.ReadConfig<ModConfig>().SoilPropertyCount;
			List<Formulas.CropRequirementFormula> TempFormulaList = gameHandler.Data.ReadSaveData<List<Formulas.CropRequirementFormula>>("Thrive.FormulaList");
			if (TempFormulaList == null || soilPropertiesCount <= TempFormulaList.Count)
			{
				Random rand = new Random((int)Game1.uniqueIDForThisGame);
				return Formulas.CropReqFormulas.OrderBy(_ => rand.Next()).Take(soilPropertiesCount).ToList();
			}
			return TempFormulaList;
		}

		public SoilPropertiesMap StartMap()
		{
			int width = Game1.currentLocation.Map.Layers[0].LayerWidth;
			int height = Game1.currentLocation.Map.Layers[0].LayerHeight;
			Random rand = new Random();
			// 10-30 is beginning mana, remember to rebalance
			return new SoilPropertiesMap(width, height, rand.Next(10, 30));
		}

		public void LoadCurrentMap()
		{
			CurrentMapKey = Game1.currentLocation.Name;
			CurrentMap = gameHandler.Data.ReadSaveData<SoilPropertiesMap>(CurrentMapKey);
		}

		public void SaveCurrentMap()
		{
			gameHandler.Data.WriteSaveData(CurrentMapKey, CurrentMap);
		}

		// run when LocationChanged and when config.IHAVERAM is false
		public void SetCurrentMap(){
			if(curMapSaved == false){
				SaveCurrentMap();
				curMapSaved = true;
			}
			if(Game1.currentLocation.Name.ToLower().Contains("farm")){
				if (Game1.currentLocation.Name == CurrentMapKey)
				{
					return;
				}
				CurrentMapKey = Game1.currentLocation.Name;
				if(!gameHandler.ReadConfig<ModConfig>().IHaveRAM){ 
					var tempMap = gameHandler.Data.ReadSaveData<SoilPropertiesMap>(CurrentMapKey);
					if (tempMap != null) {
						LoadCurrentMap();
					} else {
						CurrentMap = StartMap();
					}
				}else{
					SoilPropertiesMap tempMap = AllMaps[CurrentMapKey];
					if (tempMap == null)
					{
						AllMaps[CurrentMapKey] = StartMap();
					}
				}
					curMapSaved = false;
			}
		}

		// REMINDER: Fix numbers, REMOVE MAGIC NUMBERS
		public void UpdateSoilAndCropHealth(SoilProperties sn, Domain.CropData cd)
		{
			var configs = gameHandler.ReadConfig<ModConfig>();
			for (int x = 0; x < configs.SoilPropertyCount+2; x++)
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
