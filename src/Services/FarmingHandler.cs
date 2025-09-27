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

		public List<string> SoilNutrientNames { get; set; }
		public int propertyMin { get; set; } = 0;
		public int propertyMax { get; set; } = 1000;

		// current implementation, but move to be used for only additional farms and maps 
		public List<string> SoilMapKeys { get; set; } = new();

		// current implementation where all maps are kept in memory if player indicates to
		public Dictionary<string, SoilPropertiesMap> FarmedMapsData { get; set; } = new();

		//storage for data player has discovered for crops
		public Dictionary<string, Domain.BaseCropData> KnownCropDict { get; set; }

		// formals for distribution of attributes in various mechanics
		public List<Formulas.CropRequirementFormula> CropReqFormulaList { get; set; }
		public List<Formulas.CropDepreciationFormula> CropDepFormulaList { get; set; }
		public List<Formulas.SoilInitializationFormulas> SoilInitFormulaList { get; set; }

		public Random rand { get; set; }
		public int SoilPropertiesCount { get; set; }

		public FarmingHandler(IModHelper helper, IMonitor monitor)
		{
			Monitor = monitor;
			GameHandler = helper;
			rand = new Random((int)Game1.uniqueIDForThisGame); // unique but consistent Random seed for each save file
			SoilPropertiesCount = GameHandler.ReadConfig<ModConfig>().SoilPropertyCount + 2; // +2 to include default 'iridium' and 'mana' properties
			SoilNutrientNames = new List<string> { "Nitro", "Phos", "Aera", "pH", "Microbes" };
			InitializeFormulas();
			LoadMapDataFromStorage(); 
		}

		public void InitializeFormulas()
		{
			CropReqFormulaList = Helpers.PartialFY_Shuffle(Formulas.CropReqFormulas, rand, SoilPropertiesCount);
			CropDepFormulaList = Helpers.PartialFY_Shuffle(Formulas.CropDepreFormulas, rand, SoilPropertiesCount);
			SoilInitFormulaList = Helpers.PartialFY_Shuffle(Formulas.SoilInitFormulas, rand, SoilPropertiesCount);
			KnownCropDict = new Dictionary<string, Domain.BaseCropData>();
		}

		// helper function to initialize SoilPropertiesMap with given args
		public SoilPropertiesMap StartMap(GameLocation loc, int x, int y)
		{	
			// 10-30 is beginning mana, remember to rebalance
			return new SoilPropertiesMap(loc.Map.Layers[0].LayerWidth, loc.Map.Layers[0].LayerHeight, rand.Next(10, 30), x, y);
		}

		// try to load map data from where this mod saved it. If not found, make new maps
		public void LoadMapDataFromStorage(){
			FarmedMapsData = GameHandler.Data.ReadSaveData<Dictionary<string, SoilPropertiesMap>>("Thrive.FarmedMapsData");
			if (FarmedMapsData == null)
			{
				FarmedMapsData = new();
			}
		}

		// REMINDER: Fix numbers, REMOVE MAGIC NUMBERS
		// CROP health not updated here yet!
		// health management needs to account for players changing configs for soil property counts
		public SoilProperties UpdateSoilAndCropHealth(SoilProperties sn)
		{
			Domain.BaseCropData cd = KnownCropDict[sn.CropID];
			for (int x = 0; x < SoilPropertiesCount; x++)
			{
				if (Math.Abs(sn.SoilStats[x] - cd.Requirements[x * 2]) <= Math.Abs(cd.Requirements[x * 2 + 1]))
					sn.Health[x] += 10;
				else
					sn.Health[x] -= 18;

				sn.SoilStats[x] -= cd.SoilDeprecation[x];
			}
			return sn;
		}

		// ran at DayEnd, updates all existing soil tiles in all maps according to the crops grown on them
		public void NightlySoilUpdateAll(){
				foreach (KeyValuePair<string, SoilPropertiesMap> n_SPMap in FarmedMapsData) {
				  FarmedMapsData[n_SPMap.Key].NightlyMapUpdate(KnownCropDict);
				}
				GameHandler.Data.WriteSaveData("Thrive.FarmedMapsData", FarmedMapsData);
			
		}

		// run this method when ModEntry detects hoeing was done successfully
		public void OnHoeingDone(GameLocation loc, Microsoft.Xna.Framework.Vector2 coords)
		{
			// if this is an existing location
			if (SoilMapKeys.Contains(loc.Name))
			{
				// add soil properties to according tile
				FarmedMapsData[loc.Name].AddNewHoedTile(coords, rand, SoilPropertiesCount, SoilInitFormulaList);
			}
			// check if a farm or greenhouse
			else if (loc.IsFarm || loc.Name.ToLower().Contains(" farm") ||
						loc.IsGreenhouse || loc.Name.ToLower().Contains(" greenhouse"))
			{
				// if so, this is a new location, we take note of map name and initialize a map for it
				SoilMapKeys.Add(loc.Name);
				FarmedMapsData[loc.Name] = StartMap(loc, (int)coords.X, (int)coords.Y);
			}

		}

		// harmony patch - needs map name from within StardewValley.Crop.harvest to fix
		public int OnHarvest_GetCropQuality(StardewValley.Object o, string map_name, int x, int y)
		{
			return FarmedMapsData[map_name].MapData[y, x].CropHere.GetRandomQualityFromHealth(SoilPropertiesCount);
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
