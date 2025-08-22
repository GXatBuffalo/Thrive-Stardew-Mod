using StardewModdingAPI;
using StardewModdingAPI.Events;
using Thrive.src.Services;

namespace Thrive.src
{
	internal sealed class ModEntry : Mod
	{
		public FarmingHandler fHandler { get; set; }

		private void OnDayStarted(object? sender, DayStartedEventArgs e)
		{
			if (fHandler == null) // initialize once
			{
				fHandler = new FarmingHandler(Helper, Monitor);
				fHandler.TestSetAllCropData();
				Monitor.Log("FarmingHandler initialized.", LogLevel.Info);
			}
		}

		public override void Entry(IModHelper helper)
		{

			helper.Events.GameLoop.DayStarted += OnDayStarted;
		}

	}
}
