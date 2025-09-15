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
		public SoilPropertiesMap SecondaryFarmMap { get; private set; }
		public List<string> SoilMapKeys { get; set; }	
		public string LastMapKey { get; private set; }
		bool LastMapSaved { get; set; } = false;
		// current implementation where all maps are kept in memory if player indicates to
		public Dictionary<string, SoilPropertiesMap> AllMaps { get; private set; }

		//storage for data player has discovered for crops
		public Dictionary<string, Domain.CropData> KnownCropDict { get; set; }

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
			LastMapKey = Game1.currentLocation.Name;
			SecondaryFarmMap = GameHandler.Data.ReadSaveData<SoilPropertiesMap>(LastMapKey);
		}

		public void SaveLastMap()
		{
			GameHandler.Data.WriteSaveData(LastMapKey, SecondaryFarmMap);
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
			if(GameHandler.ReadConfig<ModConfig>().IHaveRAM){
				AllMaps = GameHandler.Data.ReadSaveData<Dictionary<string, SoilPropertiesMap>>("Thrive.AllSoilPropertyMaps");
				if (AllMaps == null)
				{
					AllMaps = new();
				}
			}
		}

		// run when LocationChanged and when config.IHAVERAM is false
		public void SetCurrentMap(GameLocation oldLocation, GameLocation newLocation)
		{
			if(newLocation.DisplayName != "Farm" && (newLocation.IsFarm || newLocation.Name.ToLower().Contains(" farm"))){
				if (newLocation.Name == LastMapKey)
				{
					return;
				}
				if (LastMapSaved == false)
				{
					SaveLastMap();
					LastMapSaved = true;
				}
				LastMapKey = newLocation.Name;
				if(!GameHandler.ReadConfig<ModConfig>().IHaveRAM){ 
					var tempMap = GameHandler.Data.ReadSaveData<SoilPropertiesMap>(LastMapKey);
					if (tempMap != null) {
						LoadCurrentMap();
					} else {
						SecondaryFarmMap = StartMap();
						SoilMapKeys.Add("Thrive." + newLocation.Name.ToLower());
					}
				}
				else{
					if (!AllMaps.TryGetValue(LastMapKey, out _))
					{
						AllMaps[LastMapKey] = StartMap();
					}
				}
				LastMapSaved = false;
			}
		}

		// REMINDER: Fix numbers, REMOVE MAGIC NUMBERS
		// CROP health not updated here yet!
		public SoilProperties UpdateSoilAndCropHealth(SoilProperties sn)
		{
			var configs = GameHandler.ReadConfig<ModConfig>();
			Domain.CropData cd = KnownCropDict[sn.CropID];
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
			if (GameHandler.ReadConfig<ModConfig>().IHaveRAM) {
				foreach (KeyValuePair<string, SoilPropertiesMap> n_SPMap in AllMaps) {
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
				GameHandler.Data.WriteSaveData("Thrive.AllSoilPropertyMaps", AllMaps);
			}
			else {
				foreach (string keyName in SoilMapKeys){
					SoilPropertiesMap tempMap = GameHandler.Data.ReadSaveData<SoilPropertiesMap>(keyName);
					SoilProperties[,] curMap = tempMap.MapData;
					for (int y = tempMap.minY; y < tempMap.maxY; y++)
					{
						for (int x = tempMap.minX; x < tempMap.maxX; x++)
						{
							curMap[y, x] = UpdateSoilAndCropHealth(curMap[y, x]);
						}
					}
					tempMap.MapData = curMap;
					GameHandler.Data.WriteSaveData(keyName, tempMap);
				}
			}
		}

		//run on game load
		public void MigrateSoilSaveData(){
			if(GameHandler.ReadConfig<ModConfig>().RAMconfigFlipped){
				if (GameHandler.ReadConfig<ModConfig>().IHaveRAM == true)
				{
					//Migrate from individual maps to dict of all
				}
				else
				{
					//migrate from dict of all mapdata to individual
				}
			}
		}

		public int NewCropQuality(StardewValley.Object o, int x, int y)
		{
			KnownCropDict.TryGetValue(o.Name, out Domain.CropData cd);
			if (cd == null){
				cd = new Domain.CropData(o.ItemId, new Random((int)Game1.uniqueIDForThisGame), CropReqFormulaList, CropDepFormulaList, GameHandler.ReadConfig<ModConfig>().SoilPropertyCount);
				KnownCropDict[o.Name] = cd;
			}
			return cd.GetRandomQualityFromHealth(GameHandler.ReadConfig<ModConfig>().SoilPropertyCount);
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
