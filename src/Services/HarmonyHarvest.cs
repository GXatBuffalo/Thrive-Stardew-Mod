using HarmonyLib;
using StardewModdingAPI;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Thrive.src.Services
{
	public static class CropQuality_HarmonyPatch
	{
		public static IMonitor Monitor;

		// call this method from Entry class
		internal static void Initialize(IMonitor monitor)
		{
			Monitor = monitor;
		}

		public static IEnumerable<CodeInstruction> HarvestForage_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new(instructions);
			MethodInfo farmerProfessionsInfo = AccessTools.PropertyGetter(typeof(StardewValley.Farmer), nameof(StardewValley.Farmer.professions));
			MethodInfo myForageQualityInfo = AccessTools.Method(typeof(FarmingHandler), nameof(FarmingHandler.NewForageQuality));
			MethodInfo addPropertiesInfo = AccessTools.Method(typeof(CropQuality_HarmonyPatch), nameof(AddProperties));

			matcher.MatchStartForward(
				new CodeMatch(OpCodes.Ldfld, farmerProfessionsInfo)
				)
				.ThrowIfNotMatch($"Could not find entry point for {nameof(HarvestForage_Transpiler)}")
				.RemoveInstructionsWithOffsets(-1, 32)
				.Insert(
					new CodeInstruction(OpCodes.Call, myForageQualityInfo),
					new CodeInstruction(OpCodes.Call, addPropertiesInfo)
				);

			return matcher.InstructionEnumeration();
		}

		public static IEnumerable<CodeInstruction> HarvestCrop_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new(instructions);
			MethodInfo myCropQualityInfo = AccessTools.Method(typeof(FarmingHandler), nameof(FarmingHandler.OnHarvest_GetCropQuality));
			MethodInfo addPropertiesInfo = AccessTools.Method(typeof(CropQuality_HarmonyPatch), nameof(AddProperties));
			MethodInfo getMapNameInfo = AccessTools.PropertyGetter(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.Name)); 
			MethodInfo clampMethodInfo = AccessTools.Method(typeof(Microsoft.Xna.Framework.MathHelper),
																								nameof(Microsoft.Xna.Framework.MathHelper.Clamp),
																								new[] { typeof(int), typeof(int), typeof(int) });

			// match first loading of variable 'cropQuality'
			matcher.MatchStartForward(new CodeMatch(OpCodes.Stloc_S, (byte)14))
						 .ThrowIfNotMatch($"Could not find entry point for {nameof(HarvestCrop_Transpiler)}")
						 .Advance(-1);
			int startIndex = matcher.Pos;

			// match cropQuality = MathHelper.Clamp
			matcher.MatchStartForward(new CodeMatch(OpCodes.Call, clampMethodInfo))
						 .ThrowIfNotMatch($"Could not find end point match for {nameof(HarvestCrop_Transpiler)}")
						 .MatchStartForward(new CodeMatch(OpCodes.Stloc_S, (byte)14));
			int endIndex = matcher.Pos;

			matcher.RemoveInstructionsInRange(startIndex, endIndex + 1)
						 .InsertAndAdvance
						 (
								new CodeInstruction(OpCodes.Ldloc_1), // StardewValley.Object o,
								new CodeInstruction(OpCodes.Ldarg_0), // this. (a StardewValley.Crop object that is using Crop.harvest)
								new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Crop), "currentLocation")), // (this.)currentLocation
								new CodeInstruction(OpCodes.Callvirt, getMapNameInfo), // get string name from this.currentLocation
								new CodeInstruction(OpCodes.Ldarg_1), // xTile
								new CodeInstruction(OpCodes.Ldarg_2), // yTile
								new CodeInstruction(OpCodes.Call, myCropQualityInfo), // OnHarvest_GetCropQuality(o, map_name, xTile, yTile)
								new CodeInstruction(OpCodes.Stloc_S, 14), // store into local var cropQuality
								new CodeInstruction(OpCodes.Callvirt, addPropertiesInfo) // unfinished, placeholder
							);

			return matcher.InstructionEnumeration(); 
		}

		public static void AddProperties(){

		}
	}
}
