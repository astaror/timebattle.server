using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.World;

namespace GrainInterfaces.PublicInterfaces
{
	public interface IWorld
	{
		Task<Guid> GetSectorActorGuid(SectorCoordinates sectorCoordinates);
		Task<List<Guid>> GetAllSectorsActors();
	}
}