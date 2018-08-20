using System.Threading.Tasks;
using Entities.Player;

namespace ConnectionInterfaces
{
	public interface IPlayerHub
	{
		Task LoadPlayerData(PlayerProfile playerProfile);
	}
}