using HarmonyLib;
using StardewModdingAPI;
using System.Reflection;
using System.Reflection.Emit;

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
			MethodInfo myForageQualityInfo = AccessTools.Method(typeof(FarmingHandler), nameof(FarmingHandler.newForageQuality));
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
			MethodInfo myCropQualityInfo = AccessTools.Method(typeof(FarmingHandler), nameof(FarmingHandler.newCropQuality));
			MethodInfo addPropertiesInfo = AccessTools.Method(typeof(CropQuality_HarmonyPatch), nameof(AddProperties));


			matcher.MatchStartForward(
				new CodeMatch(OpCodes.Stloc, 14)
				)
				.ThrowIfNotMatch($"Could not find entry point for {nameof(HarvestCrop_Transpiler)}")
				.RemoveInstructionsWithOffsets(-1, 49)
				.Insert(
					new CodeInstruction(OpCodes.Call, myCropQualityInfo),
					new CodeInstruction(OpCodes.Call, addPropertiesInfo)
				);

			return matcher.InstructionEnumeration();
		}

		public static void MyForageQuality(StardewValley.Object o, int xTile, int yTile)
		{
			o.Quality = FarmingHandler.newCropQuality(o, xTile, yTile);
		}

		public static void AddProperties(){

		}
	}
}
