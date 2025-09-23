
using Microsoft.Xna.Framework;
using StardewValley.Locations;

namespace Thrive.src.Domain
{
	public class SoilPropertiesMap
	{
		public int DataVersion { get; set; }

		public double MapMana { get; set; }  // amount of Mana on the map. 
		public double ManaMax { get; set; }  // limit on Mana on the map. 

		public SoilProperties[,] MapData { get; set; }

		public Vector2 minCoords { get; set; }
		public Vector2 maxCoords { get; set; }
		
		// dictionary to hold location of magic crops using their coords, value is mana drain
		public Dictionary<(int, int), int> MagicCrops { get; set; } = new();		

		// Constructor. Parameters are map size and mana map starts with.
		public SoilPropertiesMap(int sizeX, int sizeY, int initialMana, int x, int y)
		{
			MapMana = initialMana;
			MapData = new SoilProperties[sizeY, sizeX];
			minCoords = new Vector2(x, y);
			maxCoords = new Vector2(x, y);
		}

		public void CheckMaybeUpdateNewCoords(Vector2 coords)
		{
			minCoords = Vector2.Min(minCoords, coords);
			maxCoords = Vector2.Max(maxCoords, coords);
		}

		public void AddNewHoedTile(Vector2 coords, Random rand, int propertyCount, List<Formulas.SoilInitializationFormulas> appliedFormula)
		{
			int Xcoord = (int)coords.X;
			int Ycoord = (int)coords.Y;
			if(MapData[Xcoord, Ycoord] is null){
				MapData[Xcoord, Ycoord] = new SoilProperties(rand, propertyCount, Xcoord, Ycoord, appliedFormula);
				CheckMaybeUpdateNewCoords(coords);
			}
		}

		public void NightlyMapUpdate(Dictionary<string, Domain.BaseCropData> CropDict)
		{
			int minX = (int)minCoords.X;
			int minY = (int)minCoords.Y;
			int maxX = (int)maxCoords.X;
			int maxY = (int)maxCoords.Y;
			for (int i = minY; i <= maxY; i++)
			{
				for (int j = minX; j <= maxX; j++)
				{
					MapData[i, j].UpdateSoilandCropHealth(CropDict);
				}
			}
		}

		public void AddMagicCrop(int x, int y, int manaCost) => MagicCrops[(x, y)] = manaCost;
		public void RemoveMagicCrop(int x, int y) => MagicCrops.Remove((x, y));

		public void AddMana(int m) => MapMana = Math.Min(MapMana + m, ManaMax);
		public void RemoveMana(int m) => MapMana = Math.Max(MapMana - m, 0);
	}
}
