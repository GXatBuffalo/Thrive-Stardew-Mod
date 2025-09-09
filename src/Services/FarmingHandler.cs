using StardewModdingAPI;
using StardewValley;
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
		public Dictionary<string, Domain.CropData> KnownCropDict { get; set; }
		public List<Formulas.CropRequirementFormula> CropReqFormulaList { get; set; }
		public List<Formulas.CropDepreciationFormula> CropDepFormulaList { get; set; }
		public List<Formulas.SoilInitializationFormulas> SoilInitFormulaList { get; set; }

		public FarmingHandler(IModHelper helper, IMonitor monitor)
		{
			Monitor = monitor;
			gameHandler = helper;
			Random rand = new Random((int)Game1.uniqueIDForThisGame);
			InitializeFormulas();
		}

		public void InitializeFormulas()
		{
			int soilPropertiesCount = gameHandler.ReadConfig<ModConfig>().SoilPropertyCount;
			Random rand = new Random((int)Game1.uniqueIDForThisGame);
			CropReqFormulaList = Helpers.PartialFY_Shuffle(Formulas.CropReqFormulas, rand, soilPropertiesCount);
			CropDepFormulaList = Helpers.PartialFY_Shuffle(Formulas.CropDepreFormulas, rand, soilPropertiesCount);
			SoilInitFormulaList = Helpers.PartialFY_Shuffle(Formulas.SoilInitFormulas, rand, soilPropertiesCount);
			KnownCropDict = new Dictionary<string, Domain.CropData>();
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
		public void SetCurrentMap(GameLocation oldLocation, GameLocation newLocation)
		{
			if(curMapSaved == false){
				SaveCurrentMap();
				curMapSaved = true;
			}
			if(newLocation.IsFarm || newLocation.Name.ToLower().Contains(" farm")){
				if (newLocation.Name == CurrentMapKey)
				{
					return;
				}
				CurrentMapKey = newLocation.Name;
				if(!gameHandler.ReadConfig<ModConfig>().IHaveRAM){ 
					var tempMap = gameHandler.Data.ReadSaveData<SoilPropertiesMap>(CurrentMapKey);
					if (tempMap != null) {
						LoadCurrentMap();
					} else {
						CurrentMap = StartMap();
					}
				}
				else{
					if (!AllMaps.TryGetValue(CurrentMapKey, out _))
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

		public int NewCropQuality(StardewValley.Object o, int x, int y)
		{
			KnownCropDict.TryGetValue(o.Name, out Domain.CropData cd);
			if (cd == null){
				cd = new Domain.CropData(o.ItemId, new Random((int)Game1.uniqueIDForThisGame), CropReqFormulaList, CropDepFormulaList, gameHandler.ReadConfig<ModConfig>().SoilPropertyCount);
				KnownCropDict[o.Name] = cd;
			}
			return cd.GetRandomQualityFromHealth(gameHandler.ReadConfig<ModConfig>().SoilPropertyCount);
		}

		public static int NewForageQuality(StardewValley.Object o, int x, int y)
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
