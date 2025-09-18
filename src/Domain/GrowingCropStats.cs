using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Thrive.src.Domain
{
	public class GrowingCropStats
	{
		public string cropid { get; set; }
		public List<int> HealthStats { get; set; }


		public GrowingCropStats(string cid, List<int> StarterHealthStats, int soilPropertyCount) 
		{
			cropid = cid;
			HealthStats = StarterHealthStats.GetRange(0, soilPropertyCount);
		}

		public int GetRandomQualityFromHealth(int soilProperties)
		{
			int counter = 0;
			int average = 0;
			for (int i = 0; i < soilProperties; i++)
			{
				if (HealthStats[i] >= 100)
				{
					counter++;
					average += HealthStats[i];
				}
			}
			double percent = counter / soilProperties * 4;
			average = average / soilProperties;
			int flevel = Game1.player.GetSkillLevel(0);

			int quality = (int)Math.Clamp(average / 25, 1, 4);
			return quality;
		}
	}


}