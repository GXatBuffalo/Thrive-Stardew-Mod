using StardewModdingAPI;
using StardewNutrition.Services;

namespace Stardew_Nutrition
{
	internal sealed class ModEntry : Mod
	{
		public FarmingHandler nHandler { get; set; }

		public override void Entry(IModHelper helper)
		{
			nHandler = new FarmingHandler(helper);
			// Hook events here later
		}
	}
}
