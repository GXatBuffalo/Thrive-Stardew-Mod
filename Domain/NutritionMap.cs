using System;
using System.Collections.Generic;

namespace StardewNutrition.Domain
{
	public class NutritionMap
	{
		public int ManaMax { get; set; }
		public int MapMana { get; set; }
		public SoilNutrition[][] MapData { get; set; }
		public Dictionary<(int, int), int> MagicCrops { get; set; }//<(xcord, ycord), manacost>

		public NutritionMap(int sizeX, int sizeY, int initialMana)
		{
			MapMana = initialMana;
			MapData = new SoilNutrition[sizeX][];
			for (int i = 0; i < sizeX; i++)
				MapData[i] = new SoilNutrition[sizeY];

			MagicCrops = new Dictionary<(int, int), int>();
		}

		public void AddMagicCrop(int x, int y, int manaCost) => MagicCrops[(x, y)] = manaCost;
		public void RemoveMagicCrop(int x, int y) => MagicCrops.Remove((x, y));

		public void AddMana(int m) => MapMana = Math.Min(MapMana + m, ManaMax);
		public void RemoveMana(int m) => MapMana = Math.Max(MapMana - m, 0);
	}
}
