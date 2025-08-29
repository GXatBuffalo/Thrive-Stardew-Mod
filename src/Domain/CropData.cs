﻿using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Thrive.src;

namespace Thrive.src.Domain
{
	public class CropData
	{
		/// NOTE: The formulas are strictly for LOGIC purposes at the current time. The numbers and formula is likely to be fiddled with before actual releases.

		public List<int> Stats { get; set; } = new List<int> { 100, 100, 100, 100, 100 };

		public List<double> Requirements { get; set; }

		public List<double> SoilDeprecation { get; set; }
		public bool isMagic { get; set; } = false;

		public CropData(string seedID, Random rand)
		{
			Game1.cropData.TryGetValue(seedID, out var seedData);
			Game1.objectData.TryGetValue(seedData.HarvestItemId, out var produceData);
			//reminder - crop attributes: growth time, XP, Energy, Health, Base Price, category, seasons, multiharvest
			int price = produceData.Price;
			int edibility = produceData.Edibility is 300 or 0 ? rand.Next(1, 300) : Math.Abs(produceData.Edibility);
			float xp = (float)(16.0 * Math.Log(0.018 * price + 1.0, Math.E));  //default stardew xp formula
			int growthphase = seedData.DaysInPhase.Sum();

			double crop_factor = (Math.Sqrt(price * (edibility * 3.625) * xp * 3) / growthphase);
			isMagic = produceData.ContextTags.Any(s => s.Contains("magic", StringComparison.OrdinalIgnoreCase));

			SoilDeprecation = new List<double>
						{
								crop_factor,
								(Math.Sqrt(200/crop_factor) - 3.0)*1.2,
								rand.NextDouble()*2 - 1,
								1,
								isMagic ? 1 : 0
						};

			Requirements = new List<double>
						{
								Math.Sqrt(200/crop_factor)*16, SoilDeprecation[0],
								(xp*edibility+rand.NextDouble()*20-10)%100, SoilDeprecation[1],
								rand.Next(0, 100), SoilDeprecation[0] + SoilDeprecation[1] + SoilDeprecation[2],
								1, 0, 
								isMagic ? 1 : 0, 0
						};
		}
	}
}
