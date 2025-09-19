using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Thrive.src.Services
{
	public static class CropQuality_HarmonyPatch
	{
		private static IMonitor Monitor;

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

			// match first loading of variable 'cropQuality'
			matcher.MatchStartForward(new CodeMatch(OpCodes.Stloc_S, (byte)14))
						 .ThrowIfNotMatch($"Could not find entry point for {nameof(HarvestCrop_Transpiler)}")
						 .Advance(-1);
			int startIndex = matcher.Pos;

			matcher.MatchStartForward(new CodeMatch(OpCodes.Stloc_S, (byte)14))
			 .MatchStartForward(new CodeMatch(OpCodes.Stloc_S, (byte)14))
			 .MatchStartForward(new CodeMatch(OpCodes.Stloc_S, (byte)14))
			 .MatchStartForward(new CodeMatch(OpCodes.Stloc_S, (byte)14))
			 .ThrowIfNotMatch($"Could not find end point match for {nameof(HarvestCrop_Transpiler)}");

			int endIndex = matcher.Pos;

			matcher.RemoveInstructionsInRange(startIndex, endIndex + 1)
						 .InsertAndAdvance
						 (
								new CodeInstruction(OpCodes.Ldloc_1),
								new CodeInstruction(OpCodes.Ldarg_0),
								new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StardewValley.Crop), "currentLocation")),
								new CodeInstruction(OpCodes.Callvirt, getMapNameInfo),
								new CodeInstruction(OpCodes.Ldarg_1),
								new CodeInstruction(OpCodes.Ldarg_2),
								new CodeInstruction(OpCodes.Call, myCropQualityInfo),
								new CodeInstruction(OpCodes.Stloc_S, 14),
								new CodeInstruction(OpCodes.Call, addPropertiesInfo)
							);

			return matcher.InstructionEnumeration();
		}

		public static void AddProperties(){

		}
	}
}
