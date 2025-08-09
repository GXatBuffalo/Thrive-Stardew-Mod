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

	public class NutritionMap {
		public int mapMana { get; set; }
		public SoilNutrition[,] mapData { get; set; }
		public Dictionary<ValueTuple,int> magicCrops { get; set; }

		public NutritionMap (int sizeX, int sizeY, int mana){
			mapMana = mana;
			mapData = new SoilNutrition [sizeX, sizeY];
			magicCrops = new Dictionary<ValueTuple,int> ();
		}
	}
	public class nutritionHandler {
		public int nutriMin = 0;
		public int nutriMax = 100;
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

}