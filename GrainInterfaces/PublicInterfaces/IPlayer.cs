using System;
using System.Threading.Tasks;
using Entities.Player;
using Entities.World;

namespace GrainInterfaces.PublicInterfaces
{
	public interface IPlayer
	{
		Task<PlayerProfile> GetPlayerProfile();
		Task<Boolean> SetPlayerName(String playerName);
		Task<Boolean> Command_MoveTo(InSectorCoordinates newPosition);
		Task LoginPlayer();
	}
}