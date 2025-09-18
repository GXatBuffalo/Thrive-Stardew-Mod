using System;
using System.Collections.Generic;

namespace Thrive.src.Domain
{
	public class SoilPropertiesMap
	{
		public int DataVersion { get; set; }

		public int MapMana { get; set; }  // amount of Mana on the map. 
		public int ManaMax { get; set; }  // limit on Mana on the map.

		public SoilProperties[,] MapData { get; set; }
		// only change when a SoilProperty is initialized or removed.
		public int minY { get; set; }			// lowest tile on Y axis farmer has hoed;
		public int maxY { get; set; }     // highest tile on Y axis farmer has hoed;
		public int minX { get; set; }     // lowest tile on X axis farmer has hoed;
		public int maxX	{  get; set; }    // highest tile on X axis farmer has hoed;
		
		// dictionary to hold location of magic crops using their coords, value is mana drain
		public Dictionary<(int, int), int> MagicCrops { get; set; } = new();		

		// Constructor. Parameters are map size and mana map starts with.
		public SoilPropertiesMap(int sizeX, int sizeY, int initialMana)
		{
			MapMana = initialMana;
			MapData = new SoilProperties[sizeY, sizeX];
			minX = 0;
			maxX = sizeX-1;
			minY = 0; 
			maxY = sizeY-1;
		}

		public void AddMagicCrop(int x, int y, int manaCost) => MagicCrops[(x, y)] = manaCost;
		public void RemoveMagicCrop(int x, int y) => MagicCrops.Remove((x, y));

		public void AddMana(int m) => MapMana = Math.Min(MapMana + m, ManaMax);
		public void RemoveMana(int m) => MapMana = Math.Max(MapMana - m, 0);
	}
}
