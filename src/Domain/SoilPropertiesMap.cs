using System;
using System.Collections.Generic;

namespace Thrive.src.Domain
{
	public class SoilPropertiesMap
	{
		public int DataVersion { get; set; }
		public int ManaMax { get; set; }
		public int MapMana { get; set; }
		public SoilProperties[][] MapData { get; set; }
		public Dictionary<(int, int), int> MagicCrops { get; set; } = new();

		public SoilPropertiesMap(int sizeX, int sizeY, int initialMana)
		{
			MapMana = initialMana;
			MapData = new SoilProperties[sizeY][];
			for (int i = 0; i < sizeY; i++)
				MapData[i] = new SoilProperties[sizeX];
		}

		public void AddMagicCrop(int x, int y, int manaCost) => MagicCrops[(x, y)] = manaCost;
		public void RemoveMagicCrop(int x, int y) => MagicCrops.Remove((x, y));

		public void AddMana(int m) => MapMana = Math.Min(MapMana + m, ManaMax);
		public void RemoveMana(int m) => MapMana = Math.Max(MapMana - m, 0);
	}
}
