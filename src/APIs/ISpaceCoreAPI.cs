using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;

namespace Thrive.src.APIs
{
	public interface ISpaceCoreApi
	{
		// Must have [XmlType("Mods_SOMETHINGHERE")] attribute (required to start with "Mods_")
		void RegisterSerializerType(Type type);

	}
}