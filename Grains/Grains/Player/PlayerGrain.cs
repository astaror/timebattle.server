using System;
using System.Threading.Tasks;
using Entities.Player;
using Entities.World;
using GrainInterfaces.GrainInterfaces;
using Orleans;
using Orleans.Providers;

namespace Grains.Grains.Player
{
	[StorageProvider(ProviderName = "player")]
	public partial class PlayerGrain : Grain<PlayerProfile>, IPlayerGrain
	{
		public Task<PlayerProfile> GetPlayerProfile()
		{
			return Task.FromResult(State);
		}

		public Task<Boolean> SetPlayerName(String playerName)
		{
			if (String.IsNullOrEmpty(State.Name))
			{
				State.Name = playerName;
				WriteStateAsync();
				return Task.FromResult(true);
			}

			return Task.FromResult(false);
		}

		public async Task LoginPlayer()
		{
			// Spawn player on previous or default position
			var world = GrainFactory.GetGrain<IWorldGrain>(Guid.Empty);

			if (State.CurrentSector == null) State.CurrentSector = new SectorCoordinates(0, 0, 0);

			var sectorGuid =await  world.GetSectorActorGuid(State.CurrentSector);
			var sector     = GrainFactory.GetGrain<ISectorGrain>(sectorGuid);

			var inSectorCoordinates = State.InSector ?? await sector.GetSafeCoordinates();

			var (spawned, position) = await sector.SpawnPlayer(this.GetPrimaryKey(), inSectorCoordinates);
			if (spawned)
			{
				State.InSector = position;
			}
			else
			{
				inSectorCoordinates = await sector.GetSafeCoordinates();
				(_, position) = await sector.SpawnPlayer(this.GetPrimaryKey(), inSectorCoordinates);
				State.InSector = position;
			}
			await WriteStateAsync();
		}

		public override async Task OnActivateAsync()
		{
			await base.OnActivateAsync();
		}

		public override Task OnDeactivateAsync()
		{
			return base.OnDeactivateAsync();
		}
	}
}