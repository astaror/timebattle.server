using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.World;

namespace GrainInterfaces.ManagementInterfaces
{
	public interface IWorldManagement
	{
		Task<Dictionary<SectorCoordinates, Sector>> GetAllSectors();
		Task<Sector> GetSector(SectorCoordinates coordinates);
		Task<Boolean> CreateSector(SectorCoordinates coordinates, Sector sector);
		Task<Boolean> UpdateSector(SectorCoordinates coordinates, Sector value);
		Task<Boolean> DeleteSector(SectorCoordinates coordinates);
	}
}