using System;
using System.Threading.Tasks;
using ConnectionInterfaces;
using Entities.Player;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientProtocol
{
	public class PlayerHubClient : BaseHubClient, IPlayerHub
	{
		public PlayerHubClient(String url) : base(url)
		{
		}

		protected override String HubIdentifier() => "PlayerHub";

		public delegate void PlayerDataLoaded(PlayerProfile playerProfile);
		public event PlayerDataLoaded OnPlayerDataLoaded;

		public override Task Initialize(Guid playerId)
		{
			Connection.On<PlayerProfile>("LoadPlayerData", playerProfile => LoadPlayerData(playerProfile));
			return base.Initialize(playerId);
		}

		public Task LoadPlayerData(PlayerProfile playerProfile)
		{
			OnPlayerDataLoaded?.Invoke(playerProfile);
			return Task.CompletedTask;
		}

		public async Task Login(Guid playerGuid)
		{
			await Connection.InvokeAsync("LoginPlayer", playerGuid);
		}
	}
}