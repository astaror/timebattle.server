using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.World;
using GrainInterfaces.GrainInterfaces;
using Grains.GrainStates;
using Orleans;
using Orleans.Providers;

namespace Grains.Grains.World
{
	[StorageProvider(ProviderName = "world")]
	public partial class WorldGrain : Grain<WorldGrainState>, IWorldGrain
	{
		public async Task LoadWorld()
		{
			foreach (var worldSectorsKey in State.WorldData.WorldSectors.Keys) await StartSector(worldSectorsKey);

			await WriteStateAsync();
		}

		public Task<Guid> GetSectorActorGuid(SectorCoordinates sectorCoordinates)
		{
			return State.SectorGrains.ContainsKey(sectorCoordinates) ? Task.FromResult(State.SectorGrains[sectorCoordinates]) : Task.FromResult(Guid.Empty);
		}

		public Task<List<Guid>> GetAllSectorsActors()
		{
			return Task.FromResult(State.SectorGrains.Values.ToList());
		}

		public override async Task OnActivateAsync()
		{
			await InitializeState();
			await base.OnActivateAsync();
		}

		public override async Task OnDeactivateAsync()
		{
			await base.OnDeactivateAsync();
		}

		/// <summary>
		///     Initialize sector
		/// </summary>
		/// <param name="worldSectorCoordinates"></param>
		/// <returns></returns>
		private async Task StartSector(SectorCoordinates worldSectorCoordinates)
		{
			Guid sectorGrainId;
			if (State.SectorGrains.ContainsKey(worldSectorCoordinates))
			{
				sectorGrainId = State.SectorGrains[worldSectorCoordinates];
			}
			else
			{
				sectorGrainId = Guid.NewGuid();
				State.SectorGrains.Add(worldSectorCoordinates, sectorGrainId);
			}

			var sectorGrain = GrainFactory.GetGrain<ISectorGrain>(sectorGrainId);
			await sectorGrain.LoadSector(State.WorldData.WorldSectors[worldSectorCoordinates], worldSectorCoordinates);
			await WriteStateAsync();
		}

		/// <summary>
		///     Remove sector grain
		/// </summary>
		/// <param name="worldSectorCoordinates"></param>
		/// <returns></returns>
		protected async Task DestroySector(SectorCoordinates worldSectorCoordinates)
		{
			if (State.SectorGrains.ContainsKey(worldSectorCoordinates))
			{
				var sectorGrainId = State.SectorGrains[worldSectorCoordinates];
				var sectorGrain   = GrainFactory.GetGrain<ISectorGrain>(sectorGrainId);
				await sectorGrain.UnloadSector();
				State.SectorGrains.Remove(worldSectorCoordinates);
				await WriteStateAsync();
			}
		}

		private async Task InitializeState()
		{
			if (State.WorldData == null) State.WorldData = new Entities.World.World();
			if (State.SectorGrains == null) State.SectorGrains = new SectorGrainsDictionary();
			await WriteStateAsync();
		}
	}
}