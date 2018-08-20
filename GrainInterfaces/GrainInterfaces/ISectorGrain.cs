using System;
using System.Threading.Tasks;
using Entities.World;
using Orleans;

namespace GrainInterfaces.GrainInterfaces
{
	public interface ISectorGrain : IGrainWithGuidKey
	{
		Task LoadSector(Sector sectorData, SectorCoordinates coordinates);
		Task UnloadSector();

		Task<(Boolean, InSectorCoordinates)> SpawnPlayer(Guid playerGuid, InSectorCoordinates coordinates);
		Task DespawnPlayer(Guid playerGuid);
		Task<(Boolean, InSectorCoordinates)> MovePlayerTo(Guid playerGuid, InSectorCoordinates coordinates);

		Task<InSectorCoordinates> GetSafeCoordinates();
	}
}