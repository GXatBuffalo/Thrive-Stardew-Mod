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
		public int nitro { get; set; }
		public int phos { get; set; }
		public int pH { get; set; }
		public int iridium { get; set; }

		public SoilNutrition(int a, int b, int c, int d, int e)
		{
			nitro = a;
			phos = b;
			pH = c;
			iridium = d;
		}
	}

	public class NutritionMap
	{
		public int ManaMax { get; set; }
		public int MapMana { get; set; }

		public SoilNutrition[,] MapData { get; set; }

		public Dictionary<(int, int), int> MagicCrops { get; set; }

		public NutritionMap(int sizeX, int sizeY, int initialMana)
		{
			MapMana = initialMana;
			MapData = new SoilNutrition[sizeX, sizeY];
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

	public class nutritionHandler
	{

		public IModHelper gameHandler { get; }
		public int nutriMin = 0;
		public int nutriMax = 100;

		public nutritionHandler(IModHelper helper)
		{
			gameHandler = helper;
		}

		public void SetNitro(SoilNutrition dirtData, int n)
		{
			if (n > nutriMax)
			{
				dirtData.nitro = nutriMax;
			}
			else if (n < nutriMin)
			{
				dirtData.nitro = nutriMin;
			}
			else
			{
				dirtData.nitro = n;
			}
		}

		public void SetPhos(SoilNutrition dirtData, int p)
		{
			if (p > nutriMax)
			{
				dirtData.phos = nutriMax;
			}
			else if (p < nutriMin)
			{
				dirtData.phos = nutriMin;
			}
			else
			{
				dirtData.phos = p;
			}
		}

		public void SetpH(SoilNutrition dirtData, int x)
		{
			if (x > nutriMax)
			{
				dirtData.pH = nutriMax;
			}
			else if (x < nutriMin)
			{
				dirtData.pH = nutriMin;
			}
			else
			{
				dirtData.pH = x;
			}
		}

		public void SetIridium(SoilNutrition dirtData, int i)
		{
			if (i > nutriMax)
			{
				dirtData.iridium = nutriMax;
			}
			else if (i < nutriMin)
			{
				dirtData.iridium = nutriMin;
			}
			else
			{
				dirtData.iridium = i;
			}
		}
	}

	internal sealed class ModEntry : Mod
	{
		public nutritionHandler nHandler { get; set; }
		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			nHandler = new nutritionHandler(helper);
		}
	}
	
}