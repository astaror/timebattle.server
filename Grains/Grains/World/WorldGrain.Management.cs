using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.World;

namespace Grains.Grains.World
{
	public partial class WorldGrain
	{
		public async Task<Dictionary<SectorCoordinates, Sector>> GetAllSectors()
		{
			if (State.WorldData == null)
			{
				State.WorldData = new Entities.World.World();
				await WriteStateAsync();
			}

			return await Task.FromResult(State.WorldData.WorldSectors);
		}

		public Task<Sector> GetSector(SectorCoordinates coordinates)
		{
			return Task.FromResult(State.WorldData.WorldSectors[coordinates]);
		}

		public async Task<Boolean> CreateSector(SectorCoordinates coordinates, Sector sector)
		{
			if (!State.WorldData.WorldSectors.ContainsKey(coordinates))
			{
				State.WorldData.WorldSectors[coordinates] = sector;
				await StartSector(coordinates);
				await WriteStateAsync();
				return await Task.FromResult(true);
			}

			return await Task.FromResult(false);
		}

		public async Task<Boolean> UpdateSector(SectorCoordinates coordinates, Sector value)
		{
			if (State.WorldData.WorldSectors.ContainsKey(coordinates))
			{
				State.WorldData.WorldSectors[coordinates] = value;
				await StartSector(coordinates);
				await WriteStateAsync();
				return await Task.FromResult(true);
			}

			return await Task.FromResult(false);
		}

		public async Task<Boolean> DeleteSector(SectorCoordinates coordinates)
		{
			if (State.WorldData.WorldSectors.ContainsKey(coordinates))
			{
				await DestroySector(coordinates);
				State.WorldData.WorldSectors.Remove(coordinates);
				await WriteStateAsync();
				return await Task.FromResult(true);
			}

			return await Task.FromResult(false);
		}
	}
}