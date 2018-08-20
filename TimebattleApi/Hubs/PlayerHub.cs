using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectionInterfaces;
using Entities.Player;
using GrainInterfaces.GrainInterfaces;
using Microsoft.AspNetCore.SignalR;
using Orleans;

namespace TimebattleApi.Hubs
{
	public class PlayerHub : Hub<IPlayerHub>
	{
		private readonly IClusterClient _clusterClient;
		private readonly Dictionary<String, Guid> _connectionToPlayer;

		public PlayerHub(IClusterClient clusterClient)
		{
			_clusterClient = clusterClient;
			_connectionToPlayer = new Dictionary<String, Guid>();
		}

		public Task IdentifyPlayer(Guid playerId)
		{
			_connectionToPlayer[Context.ConnectionId] = playerId;
			return Task.CompletedTask;
		}

		public async Task LoginPlayer(Guid playerId)
		{
			var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(playerId);
			await playerGrain.LoginPlayer();
			await Clients.Caller.LoadPlayerData(await playerGrain.GetPlayerProfile());
		}
	}
}