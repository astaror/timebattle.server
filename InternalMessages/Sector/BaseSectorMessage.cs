using System;
using Entities.World;

namespace InternalMessages.Sector
{
	public class BaseSectorMessage
	{
		public SectorCoordinates SectorCoordinates { get; set; }
	}

	public class BaseSectorPlayerMessage : BaseSectorMessage
	{
		public Guid PlayerId { get; set; }
	}

	public class SectorPlayerLeft : BaseSectorPlayerMessage {}
	public class SectorPlayerEnter : BaseSectorPlayerMessage {}

	public class SectorPlayerLeftPosition : BaseSectorPlayerMessage
	{
		public InSectorCoordinates Position { get; set; }
	}
	public class SectorPlayerSetPosition : BaseSectorPlayerMessage
	{
		public InSectorCoordinates Position { get; set; }
	}
}