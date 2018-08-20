using System;
using System.Collections.Generic;
using Entities.World;
using Newtonsoft.Json;

namespace Grains.GrainStates
{
	public class WorldGrainState
	{
		public WorldGrainState()
		{
			WorldData = new Entities.World.World();
			SectorGrains = new SectorGrainsDictionary();
		}

		// TODO: Store world data in different database, not in grain state
		public Entities.World.World   WorldData    { get; set; }
		public SectorGrainsDictionary SectorGrains { get; set; }
	}

	[JsonArray]
	public class SectorGrainsDictionary : Dictionary<SectorCoordinates, Guid>
	{
	}
}