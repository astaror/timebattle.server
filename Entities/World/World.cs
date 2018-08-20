using System.Collections.Generic;
using Newtonsoft.Json;

namespace Entities.World
{
	public class World
	{
		public WorldSectorsDictionary WorldSectors { get; protected set; }

		public World(WorldSectorsDictionary worldSectors)
		{
			WorldSectors = worldSectors;
		}

		public World()
		{
			WorldSectors = new WorldSectorsDictionary();
		}
	}

	[JsonArray]
	public class WorldSectorsDictionary : Dictionary<SectorCoordinates, Sector> {}
}