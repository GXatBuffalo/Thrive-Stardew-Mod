using System;
using System.Collections.Generic;
using StardewValley;

namespace StardewNutrition.Domain
{
	public class CropData
	{
		/// NOTE: The formulas are strictly for LOGIC purposes at the current time. The numbers and formula is likely to be fiddled with before actual releases.
		// placeholder, base stats for nutrition when eaten. 
		public List<int> Stats { get; set; } = new List<int> { 100, 100, 100, 100, 100 };
		// requirements and their range. even numbers, base number, odd is range.
		public List<int> Requirements { get; set; }
		// how much to increase or decrease soil quality/stats by each day
		public List<int> SoilDeprecation { get; set; }
		public bool isMagic { get; set; } = false;

		public CropData(string seedID)
		{
			Random rand = new Random();
			Game1.cropData.TryGetValue(seedID, out var seedData);
			Game1.objectData.TryGetValue(seedData.HarvestItemId, out var produceData);
			//reminder - crop attributes: growth time, XP, Energy, Health, Base Price, category, seasons, multiharvest
			int price = produceData.Price;
			int edibility = Math.Abs(produceData.Edibility);
			float xp = (float)(16.0 * Math.Log(0.018 * price + 1.0, Math.E));  //default stardew xp formula
			int growthphase = seedData.DaysInPhase[seedData.DaysInPhase.Count - 1];

			int crop_factor = (int)(price * (edibility * 3.625) * xp / growthphase / 100 - 3 + (rand.NextDouble() * 3 - 2));

			SoilDeprecation = new List<int>
						{
								rand.Next(0, crop_factor) - 4,
								rand.Next(0, crop_factor) - 4,
								rand.Next(0, crop_factor) - 4,
								rand.Next(0, crop_factor) - 4,
								rand.Next(0, crop_factor) - 4
						};

			Requirements = new List<int>
						{
								edibility, SoilDeprecation[0],
								(int)xp, SoilDeprecation[1],
								rand.Next(0, 100), SoilDeprecation[0] + SoilDeprecation[1] + SoilDeprecation[2],
								1, 0, 
								isMagic ? 1 : 0, 0
						};
		}
	}
}
