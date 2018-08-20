using System;
using System.Threading.Tasks;
using Entities.World;
using Orleans;

namespace GrainInterfaces.Observers
{
	public interface ISectorObserver
	{
		Task PlayerLeftPosition(SectorCoordinates coordinates, Guid playerId, InSectorCoordinates position);
		Task PlayerSetPosition(SectorCoordinates coordinates, Guid playerId, InSectorCoordinates position);
		Task PlayerEntersSector(SectorCoordinates coordinates, Guid playerId);
		Task PlayerLeftSector(SectorCoordinates coordinates, Guid playerId);
	}
}