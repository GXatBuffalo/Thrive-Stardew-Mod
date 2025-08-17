using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewNutrition.Services;

namespace Stardew_Nutrition
{
	internal sealed class ModEntry : Mod
	{
		public FarmingHandler fHandler { get; set; }

		private void OnDayStarted(object? sender, DayStartedEventArgs e)
		{
			if (fHandler == null) // initialize once
			{
				fHandler = new FarmingHandler(this.Helper);
				fHandler.TestSetAllCropData();
				this.Monitor.Log("FarmingHandler initialized.", LogLevel.Info);
			}
		}

		public override void Entry(IModHelper helper)
		{

			helper.Events.GameLoop.DayStarted += this.OnDayStarted;
		}

	}
}
