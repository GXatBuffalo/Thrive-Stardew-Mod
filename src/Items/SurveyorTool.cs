using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System.Xml.Serialization;

namespace Thrive.src.Items
{
	[XmlType("Mods_SpaceCore_Thrive")]
	internal class SurveyorTool: Tool
	{
		public IMonitor Monitor { get; }
		public IModHelper GameHandler { get; }

		public const int standardStaminaReduction = 0;
		public static Texture2D toolTexture { get; set; }
		[XmlElement("instantUse")]
		public readonly NetBool instantUse = new NetBool(true);
		[XmlIgnore]
		private string _description { get; set; } = "Tool to check soil conditions.";
		[XmlIgnore]
		protected string displayName { get; set; } = "Surveyor";

		public SurveyorTool(IModHelper helper, IMonitor monitor)
		{
			Monitor = monitor;
			GameHandler = helper;
		}

		protected override Item GetOneNew()
		{
			throw new NotImplementedException();
		}

		public void loadDescription()
		{
			GameHandler.Translation.Get("Thrive.Surveyor.Name");
		}
		public void loadDisplayName()
		{
			GameHandler.Translation.Get("Thrive.Surveyor.Description");
		}
	}
}
