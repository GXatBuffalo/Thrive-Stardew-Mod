using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace StardewNutrition
{
	public class SoilNutrition
	{
		public string CropID { get; set; }
		public int Nitro { get; set; }
		public int Phos { get; set; }
		public int PH { get; set; }
		public int Iridium { get; set; }
		public List<int> Health { get; set; } = new List<int> { 100, 100, 100, 100, 100 };

		public SoilNutrition(string id, int nitro, int phos, int ph, int iridium)
		{
			CropID = id;
			Nitro = nitro;
			Phos = phos;
			PH = ph;
			Iridium = iridium;
		}
	}

	public class NutritionMap
	{
		public int ManaMax { get; set; }
		public int MapMana { get; set; } = 120;
		public SoilNutrition[][] MapData { get; set; }
		public Dictionary<(int, int), int> MagicCrops { get; set; }

		public NutritionMap(int sizeX, int sizeY, int initialMana)
		{
			MapMana = initialMana;
			MapData = new SoilNutrition[sizeX][];
			for (int i = 0; i < sizeX; i++)
			{
				MapData[i] = new SoilNutrition[sizeY];
			}
			MagicCrops = new Dictionary<(int, int), int>();
		}

		public void AddMagicCrop(int x, int y, int manaCost)
		{
			MagicCrops.Add((x, y), manaCost);
		}

		public void RemoveMagicCrop(int x, int y)
		{
			MagicCrops.Remove((x, y));
		}

		public void AddMana(int m)
		{
			MapMana += m;
			if (MapMana > ManaMax)
			{
				MapMana = ManaMax;
			}
		}

		public void RemoveMana(int m)
		{
			MapMana -= m;
			if (MapMana < 0)
			{
				MapMana = 0;
			}
		}
	}

	public class NutritionHandler
	{
		public IModHelper gameHandler { get; }
		public int nutriMin = 0;
		public int nutriMax = 1000;
		public NutritionMap CurrentMap {  get; private set; }
		public string CurrentKey { get; private set; }

		public NutritionHandler(IModHelper helper){ 
		{
			gameHandler = helper;
		}

		private int rangeLimitHelper(int value, int min, int max)
		{
			return Math.Min(Math.Max(value, min), max);
		}

		public void SetNitro(SoilNutrition dirtData, int n)
		{
			dirtData.Nitro = rangeLimitHelper(n, nutriMin, nutriMax);
		}

		public void SetPhos(SoilNutrition dirtData, int p)
		{
			dirtData.Phos = rangeLimitHelper(p, nutriMin, nutriMax);
		}

		public void SetpH(SoilNutrition dirtData, int x)
		{
			dirtData.PH = rangeLimitHelper(x, nutriMin, nutriMax);
		}

		public void SetIridium(SoilNutrition dirtData, int i)
		{
			dirtData.Iridium = rangeLimitHelper(i, nutriMin, nutriMax);
		}

		public void starterMap(){
			int width = Game1.currentLocation.Map.Layers[0].LayerWidth;
			int height = Game1.currentLocation.Map.Layers[0].LayerHeight;

			Random rand = new Random();

			CurrentMap = new NutritionMap(width, height, rand.Next(10,30));
			CurrentKey = Game1.currentLocation.Name;
		}

		public void getCurrentMap()
		{
			CurrentKey = Game1.currentLocation.Name;
			CurrentMap = gameHandler.Data.ReadSaveData<NutritionMap>(CurrentKey);
		}

		public void SaveCurrentMap()
		{
			gameHandler.Data.WriteSaveData(CurrentKey, CurrentMap);
		}

		//seems inefficient. The 4 stats in SoilNutrition shall be saved in a list going forward.
		public void UpdateSoilAndCropHealth(SoilNutrition sn, CropData cd){
			if (Math.Abs(sn.Nitro - cd.Requirements[0]) <= Math.Abs(cd.Requirements[1])){
				sn.Health[0] += 10;
			}
			else{
				sn.Health[0] -= 10;
			}
			if (Math.Abs(sn.Phos - cd.Requirements[2]) <= Math.Abs(cd.Requirements[3]))
			{
				sn.Health[1] += 10;
			}
			else
			{
				sn.Health[1] -= 10;
			}
			if (Math.Abs(sn.PH - cd.Requirements[4]) <= Math.Abs(cd.Requirements[5]))
			{
				sn.Health[2] += 10;
			}
			else
			{
				sn.Health[2] -= 10;
			}
			if (Math.Abs(sn.Iridium - cd.Requirements[6]) <= Math.Abs(cd.Requirements[7]))
			{
				sn.Health[3] += 10;
			}
			else
			{
				sn.Health[3] -= 10;
			}
			sn.Nitro -= cd.SoilDeprecation[0];
			sn.Phos -= cd.SoilDeprecation[1];
			sn.PH -= cd.SoilDeprecation[2];
			sn.Iridium -= cd.SoilDeprecation[3];
		}
		}

	public class CropData
	{
		/// NOTE: The formulas are strictly for LOGIC purposes at the current time. The numbers and formula is likely to be fiddled with before actual releases.
		// placeholder, base stats for nutrition when eaten. 
		public List<int> Stats { get; set; } = new List<int> { 100, 100, 100, 100, 100 };
		// requirements and their range. even numbers, base number, odd is range.
		public List<int> Requirements { get; set; } 
		// how much to increase or decrease soil quality/stats by each day
		public List<int> SoilDeprecation { get; set; }
		public Boolean isMagic { get; set; } = false;

		public CropData(string seedID){
			Random rand = new Random();
			//risk of null errors, but intent is to use function in a way that goes around.
			Game1.cropData.TryGetValue(seedID, out StardewValley.GameData.Crops.CropData seedData);
			Game1.objectData.TryGetValue(seedData.HarvestItemId, out StardewValley.GameData.Objects.ObjectData produceData);
			//reminder - crop attributes: growth time, XP, Energy, Health, Base Price, category, seasons, multiharvest
			int price = produceData.Price;
			int edibility = Math.Abs(produceData.Edibility);
			float xp = (float)(16.0 * Math.Log(0.018 * (double)price + 1.0, Math.E)); //default stardew xp formula
			int growthphase = seedData.DaysInPhase[seedData.DaysInPhase.Count - 1];

			int crop_factor = ((int)(price * (edibility * 3.625) * xp / growthphase / 100 - 3 + (rand.NextDouble()*3-2)));
			SoilDeprecation = new List<int>{ rand.Next(0, crop_factor) - 4, rand.Next(0, crop_factor) - 4, rand.Next(0, crop_factor) - 4, rand.Next(0, crop_factor) - 4, rand.Next(0, crop_factor) - 4};
			Requirements = new List<int> { edibility, SoilDeprecation[0], (int)xp, SoilDeprecation[1], rand.Next(0, 100), SoilDeprecation[0] + SoilDeprecation[1] + SoilDeprecation[2], 1, 0, isMagic ? 1 : 0 , 0};
			}
	}

	internal sealed class ModEntry : Mod
	{
		public NutritionHandler nHandler { get; set; }

		public override void Entry(IModHelper helper)
		{
			nHandler = new NutritionHandler(helper);
		}
	}
	
}