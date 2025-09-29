using StardewValley;

namespace Thrive.src.Domain
{
	public class GrowingCropStats
	{
		public List<double> HealthStats { get; set; } = new();

		public GrowingCropStats(List<double> StarterHealthStats, int soilPropertyCount) 
		{
			HealthStats = StarterHealthStats.GetRange(0, soilPropertyCount); // initializes at default 0
		}

		// using the health from managed soil, return a quality for the crop growing on it
		// temp formula
		public int GetRandomQualityFromHealth(int soilProperties)
		{
			int counter = 0;
			double average = 0;
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