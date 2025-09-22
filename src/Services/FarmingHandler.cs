using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using Thrive.src.Domain;

namespace Thrive.src.Services
{
	public class FarmingHandler
	{
		public IMonitor Monitor { get; }
		public IModHelper GameHandler { get; }

		public List<string> SoilNutrientNames { get; set; } = new List<string> { "Nitro", "Phos", "Aera", "pH", "Microbes" };
		public int propertyMin = 0;
		public int propertyMax = 1000;

		//keep the main farm map always loaded
		public SoilPropertiesMap MainFarmMap { get; private set; }

		// current implementation, but move to be used for only additional farms and maps 
		public List<string> SoilMapKeys { get; set; }	

		// current implementation where all maps are kept in memory if player indicates to
		public Dictionary<string, SoilPropertiesMap> FarmedMapData { get; private set; }

		//storage for data player has discovered for crops
		public Dictionary<string, Domain.BaseCropData> KnownCropDict { get; set; }

		// formals for distribution of attributes in various mechanics
		public List<Formulas.CropRequirementFormula> CropReqFormulaList { get; set; }
		public List<Formulas.CropDepreciationFormula> CropDepFormulaList { get; set; }
		public List<Formulas.SoilInitializationFormulas> SoilInitFormulaList { get; set; }

		public FarmingHandler(IModHelper helper, IMonitor monitor)
		{
			Monitor = monitor;
			GameHandler = helper;
			InitializeFormulas();
			LoadMainFarmMap();
			LoadAllMapData();
		}

		public void InitializeFormulas()
		{
			int soilPropertiesCount = GameHandler.ReadConfig<ModConfig>().SoilPropertyCount;
			Random rand = new Random((int)Game1.uniqueIDForThisGame);
			CropReqFormulaList = Helpers.PartialFY_Shuffle(Formulas.CropReqFormulas, rand, soilPropertiesCount);
			CropDepFormulaList = Helpers.PartialFY_Shuffle(Formulas.CropDepreFormulas, rand, soilPropertiesCount);
			SoilInitFormulaList = Helpers.PartialFY_Shuffle(Formulas.SoilInitFormulas, rand, soilPropertiesCount);
			KnownCropDict = new Dictionary<string, Domain.BaseCropData>();
		}

		public SoilPropertiesMap StartMap()
		{	
			int width = Game1.currentLocation.Map.Layers[0].LayerWidth;
			int height = Game1.currentLocation.Map.Layers[0].LayerHeight;
			Random rand = new Random();
			// 10-30 is beginning mana, remember to rebalance
			return new SoilPropertiesMap(width, height, rand.Next(10, 30));
		}

		public void StartFarmMap(){
			Farm f = Game1.getFarm();
			int width = f.Map.Layers[0].LayerWidth;
			int height = f.Map.Layers[0].LayerHeight;
			Random rand = new Random();
			// 10-30 is beginning mana, remember to rebalance
			MainFarmMap =  new SoilPropertiesMap(width, height, rand.Next(10, 30));
		}

		public void LoadMainFarmMap(){
			SoilPropertiesMap tempfarm = GameHandler.Data.ReadSaveData<SoilPropertiesMap>("Thrive.MainFarm");
			if (tempfarm != null)
			{
				MainFarmMap = tempfarm;
			}else {
				StartFarmMap();
			}
		}

		public void LoadAllMapData(){
			FarmedMapData = GameHandler.Data.ReadSaveData<Dictionary<string, SoilPropertiesMap>>("Thrive.AllSoilPropertyMaps");
			if (FarmedMapData == null)
			{
				FarmedMapData = new();
			}
			
		}

		// REMINDER: Fix numbers, REMOVE MAGIC NUMBERS
		// CROP health not updated here yet!
		// health management needs to account for players changing configs for soil property counts
		public SoilProperties UpdateSoilAndCropHealth(SoilProperties sn)
		{
			var configs = GameHandler.ReadConfig<ModConfig>();
			Domain.BaseCropData cd = KnownCropDict[sn.CropHere.CropID];
			for (int x = 0; x < configs.SoilPropertyCount+2; x++)
			{
				if (Math.Abs(sn.SoilStats[x] - cd.Requirements[x * 2]) <= Math.Abs(cd.Requirements[x * 2 + 1]))
					sn.Health[x] += 10;
				else
					sn.Health[x] -= 18;

				sn.SoilStats[x] -= cd.SoilDeprecation[x];
			}
			return sn;
		}

		public void NightlySoilUpdateAll(){
				foreach (KeyValuePair<string, SoilPropertiesMap> n_SPMap in FarmedMapData) {
					SoilProperties[,] curMap = n_SPMap.Value.MapData;
					for (int y = n_SPMap.Value.minY; y < n_SPMap.Value.maxY; y++)
					{
						for (int x = n_SPMap.Value.minX; x < n_SPMap.Value.maxX; x++)
						{
							curMap[y, x] = UpdateSoilAndCropHealth(curMap[y, x]);
						}
					}
					n_SPMap.Value.MapData = curMap;
				}
				GameHandler.Data.WriteSaveData("Thrive.AllSoilPropertyMaps", FarmedMapData);
			
		}

		public void OnHoeingDone(string loc, Vector2 coords){
			if (!SoilMapKeys.Contains(loc)){
				
			}
		}


		// harmony patch - needs map name from within StardewValley.Crop.harvest to fix
		public int OnHarvest_GetCropQuality(StardewValley.Object o, string map_name, int x, int y)
		{
			SoilProperties sn = FarmedMapData[map_name].MapData[y, x];

			return sn.CropHere.GetRandomQualityFromHealth(GameHandler.ReadConfig<ModConfig>().SoilPropertyCount);
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
