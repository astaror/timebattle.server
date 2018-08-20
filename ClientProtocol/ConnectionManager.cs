using System;
using System.Threading.Tasks;

namespace ClientProtocol
{
	public class ConnectionManager : IDisposable
	{
		public PlayerHubClient PlayerHubClient { get; protected set; }
		public SectorHubClient SectorHubClient { get; protected set; }

		public ConnectionManager(String address = "https://localhost:44334")
		{
			PlayerHubClient = new PlayerHubClient(address);
			SectorHubClient = new SectorHubClient(address);
		}

		public async Task Initialize(Guid playerId)
		{
			await SectorHubClient.Initialize(playerId);
			await PlayerHubClient.Initialize(playerId);
		}

		public void Dispose()
		{
			PlayerHubClient?.Dispose();
			SectorHubClient?.Dispose();
		}
	}
}