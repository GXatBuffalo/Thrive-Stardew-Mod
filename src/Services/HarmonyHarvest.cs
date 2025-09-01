using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thrive.src.Services
{
	internal class HarmonyHarvest
	{
		private static IMonitor Monitor;

		// call this method from Entry class
		internal static void Initialize(IMonitor monitor)
		{
			Monitor = monitor;
		}
		public static void AddProperties_Transpiler(){
			try
			{
				;

			}
			catch (Exception ex)
			{
				Monitor.Log($"Failed in harmony patch {nameof(AddProperties_Transpiler)}: {ex}\n", LogLevel.Error);
			}
		}
	}
}
