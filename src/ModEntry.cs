using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Thrive.src.Services;
using StardewValley;

namespace Thrive.src
{
	internal sealed class ModEntry : Mod
	{
		public FarmingHandler F_Handler { get; set; }
		private ModConfig Config;

		private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
		{
			if (F_Handler == null) // initialize once
			{
				F_Handler = new FarmingHandler(Helper, Monitor);
				//F_Handler.TestSetAllCropData();
				Monitor.Log("FarmingHandler initialized.", LogLevel.Info);
			}
		}

		public override void Entry(IModHelper helper)
		{
			this.Config = this.Helper.ReadConfig<ModConfig>();
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
			helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
			helper.Events.Player.Warped += OnPlayerWarp;
		}

		private void OnPlayerWarp(object? sender, WarpedEventArgs e)
		{
			GameLocation oldLocation = e.OldLocation;
			GameLocation newLocation = e.NewLocation;
			F_Handler.SetCurrentMap(oldLocation, newLocation);
		}

		private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
		{
			SetConfigMenu(sender, e);
		}

		private void HarmonyPatching()
		{
			var harmony = new Harmony(this.ModManifest.UniqueID);

			harmony.Patch(
				 original: AccessTools.Method(typeof(StardewValley.Crop), nameof(Crop.harvest)),
				 transpiler: new HarmonyMethod(typeof(CropQuality_HarmonyPatch), nameof(CropQuality_HarmonyPatch.HarvestForage_Transpiler))
			);

			harmony.Patch(
				 original: AccessTools.Method(typeof(StardewValley.Crop), nameof(Crop.harvest)),
				 transpiler: new HarmonyMethod(typeof(CropQuality_HarmonyPatch), nameof(CropQuality_HarmonyPatch.HarvestCrop_Transpiler))
			);

		}

		private void SetConfigMenu(object? sender, GameLaunchedEventArgs e)
		{
			// get Generic Mod Config Menu's API (if it's installed)
			var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (configMenu is null)
				return;

			// register mod
			configMenu.Register(
					mod: this.ModManifest,
					reset: () => this.Config = new ModConfig(),
					save: () => this.Helper.WriteConfig(this.Config)
			);

			// add some config options
			configMenu.AddNumberOption(
					mod: this.ModManifest,
					getValue: () => (float)this.Config.FruitsCategoryMultiplier,
					setValue: value => this.Config.FruitsCategoryMultiplier = value,
					name: () => "Fruit Category Multiplier",
					tooltip: () => "A multiplier for how much fruits deplete or replenish soil.",
					min: 0.1f,
					max: 3.0f,
					interval: 0.05f
			);

			configMenu.AddNumberOption(
					mod: this.ModManifest,
					getValue: () => (float)this.Config.VegetableCategoryMultiplier,
					setValue: value => this.Config.VegetableCategoryMultiplier = value,
					name: () => "Vegetable Category Multiplier",
					tooltip: () => "A multiplier for how much veggies deplete or replenish soil.",
					min: 0.1f,
					max: 3.0f,
					interval: 0.05f
			);

			configMenu.AddNumberOption(
					mod: this.ModManifest,
					getValue: () => (float)this.Config.FlowersCategoryMultiplier,
					setValue: value => this.Config.FlowersCategoryMultiplier = value,
					name: () => "Flower Category Multiplier",
					tooltip: () => "A multiplier for how much flowers deplete or replenish soil.",
					min: 0.1f,
					max: 3.0f,
					interval: 0.05f
			);


			configMenu.AddNumberOption(
				mod: this.ModManifest,
				getValue: () => (float)this.Config.GreensCategoryMultiplier,
				setValue: value => this.Config.GreensCategoryMultiplier = value,
				name: () => "Greens Category Multiplier",
				tooltip: () => "A multiplier for how much 'greens'(eg. spring onion) deplete or replenish soil.",
				min: 0.1f,
				max: 3.0f,
				interval: 0.05f
		);

			configMenu.AddNumberOption(
					mod: this.ModManifest,
					getValue: () => (float)this.Config.GrowthDepletionMultiplier,
					setValue: value => this.Config.GrowthDepletionMultiplier = value,
					name: () => "Growth-Depletion Multiplier",
					tooltip: () => "An overall multiplier for how much crops deplete or replenish soil.",
					min: 0.1f,
					max: 3.0f,
					interval: 0.05f
			);

			configMenu.AddNumberOption(
					mod: this.ModManifest,
					getValue: () => (float)this.Config.RestrictionMultiplier,
					setValue: value => this.Config.RestrictionMultiplier = value,
					name: () => "Restriction Multiplier",
					tooltip: () => "A multiplier for how restrictive nutrient requirements can be for soil.",
					min: 0.1f,
					max: 3.0f,
					interval: 0.05f
			);

			configMenu.AddNumberOption(
					mod: this.ModManifest,
					getValue: () => this.Config.SoilPropertyCount,
					setValue: value => this.Config.SoilPropertyCount = value,
					name: () => "Soil Property Count",
					tooltip: () => "Number of addtional soil properties to use (iridium and mana always on by default).",
					min: 1,
					max: 5,
					interval: 1
			);

			configMenu.AddBoolOption(
				mod: this.ModManifest,
				getValue: () => this.Config.IHaveRAM,
				setValue: value => this.Config.IHaveRAM = value,
				name: () => "I have RAM.",
				tooltip: () => "I have RAM, load all my maps to memory."
			);
		}
	}
}