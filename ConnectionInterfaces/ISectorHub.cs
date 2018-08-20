using System;
using System.Threading.Tasks;
using Entities.World;

namespace ConnectionInterfaces
{
	public interface ISectorHub
	{
		Task SectorChanged(Sector sector);
		Task PlayerEnterSector(Guid playerId);
		Task PlayerLeftSector(Guid playerId);
		Task PlayerLeftPosition(Guid playerId, InSectorCoordinates position);
		Task PlayerEntersPosition(Guid playerId, InSectorCoordinates position);
	}
}