using System;
using System.Collections.Generic;
using Entities.World;

namespace Grains.GrainStates
{
	public class SectorGrainState
	{
		public Sector                   SectorData      { get; set; }
		public List<Guid>               PlayersInSector { get; set; }
		public PlayerPositionDictionary PlayerPositions { get; set; }
		public SectorCoordinates		SectorCoordinates { get; set; }
	}

	public class PlayerPositionDictionary : Dictionary<Guid, InSectorCoordinates>
	{
	}
}