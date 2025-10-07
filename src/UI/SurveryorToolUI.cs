using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thrive.src.UI
{
	internal class SurveryorToolUI
	{
		public static Texture2D toolTexture { get; set; }
		public static string toolTextureKey { get; set; }
		private Texture2D ItemTexture;

		public SurveryorToolUI() { }

		public override void drawInMenu(SpriteBatch b, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{

		}

	}
}
