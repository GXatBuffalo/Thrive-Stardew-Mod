using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace StardewNutrition
{
	public class SoilNutrition
	{
		public int Nitro { get; set; }
		public int Phos { get; set; }
		public int PH { get; set; }
		public int Iridium { get; set; }

		public SoilNutrition(int nitro, int phos, int ph, int iridium)
		{
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
		public int nutriMax = 100;
		public NutritionMap CurrentMap {  get; private set; }
		public string CurrentKey { get; private set; }

		public NutritionHandler(IModHelper helper)
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
			int widthInPixels = Game1.currentLocation.Map.DisplayWidth;
			int heightInPixels = Game1.currentLocation.Map.DisplayHeight;

			int widthInTiles = widthInPixels / Game1.tileSize;
			int heightInTiles = heightInPixels / Game1.tileSize;

			Random rand = new Random();

			CurrentMap = new NutritionMap(widthInTiles, heightInTiles, rand.Next(10,30));
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