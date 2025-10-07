using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System.Xml.Serialization;
using Thrive.src.UI;

namespace Thrive.src.Items
{
	[XmlType("Mods_SpaceCore_Thrive")]
	internal class SurveyorTool: Tool
	{
		public IMonitor Monitor { get; }
		public IModHelper GameHandler { get; }

		public const int standardStaminaReduction = 0;
		[XmlElement("instantUse")]
		public readonly NetBool instantUse = new NetBool(true);
		public SurveryorToolUI toolUI { get; set; }

		public SurveyorTool(IModHelper helper, IMonitor monitor)
		{
			Monitor = monitor;
			GameHandler = helper;
		}

		protected override Item GetOneNew()
		{
			return new SurveyorTool(this.GameHandler, this.Monitor);
		}

		protected override string loadDescription()
		{
			return GameHandler.Translation.Get("Thrive.Surveyor.Name");
		}
		protected override string loadDisplayName()
		{
			return GameHandler.Translation.Get("Thrive.Surveyor.Description");
		}

	}
}
