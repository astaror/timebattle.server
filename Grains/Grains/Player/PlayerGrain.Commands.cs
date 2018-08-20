using System;
using System.Threading.Tasks;
using Entities.World;
using GrainInterfaces.GrainInterfaces;
using Orleans;

namespace Grains.Grains.Player
{
	/// <summary>
	///     Partial for player's commands
	/// </summary>
	public partial class PlayerGrain
	{
		public async Task<Boolean> Command_MoveTo(InSectorCoordinates newPosition)
		{
			var world  = GrainFactory.GetGrain<IWorldGrain>(Guid.Empty);
			var sectorGuid = await world.GetSectorActorGuid(State.CurrentSector);
            var sector = GrainFactory.GetGrain<ISectorGrain>(sectorGuid);
            var (succeded, position) = await sector.MovePlayerTo(this.GetPrimaryKey(), newPosition);
            return succeded;
		}
	}
}