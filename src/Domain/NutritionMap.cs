using System;
using System.Collections.Generic;

namespace Thrive.src.Domain
{
	public class NutritionMap
	{
		public int ManaMax { get; set; }
		public int MapMana { get; set; }
		public SoilNutrition[][] MapData { get; set; }
		public Dictionary<(int, int), int> MagicCrops { get; set; } = new();

		public NutritionMap(int sizeX, int sizeY, int initialMana)
		{
			MapMana = initialMana;
			MapData = new SoilNutrition[sizeY][];
			for (int i = 0; i < sizeY; i++)
				MapData[i] = new SoilNutrition[sizeX];
		}

		public void AddMagicCrop(int x, int y, int manaCost) => MagicCrops[(x, y)] = manaCost;
		public void RemoveMagicCrop(int x, int y) => MagicCrops.Remove((x, y));

		public void AddMana(int m) => MapMana = Math.Min(MapMana + m, ManaMax);
		public void RemoveMana(int m) => MapMana = Math.Max(MapMana - m, 0);
	}
}
