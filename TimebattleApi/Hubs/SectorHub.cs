using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectionInterfaces;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using TimebattleApi.Services.Sector;

namespace TimebattleApi.Hubs
{
	public class SectorHub : Hub<ISectorHub>
	{
		private readonly IClusterClient           _clusterClient;
		private readonly IConnectionIdHolder _connectionIdHolder;

		public SectorHub(IClusterClient clusterClient, IConnectionIdHolder connectionIdHolder)
		{
			_clusterClient = clusterClient;
			_connectionIdHolder = connectionIdHolder;
		}

		public Task IdentifyPlayer(Guid playerId)
		{
			_connectionIdHolder.AddUserInformation(playerId, Context.ConnectionId);
			return Task.CompletedTask;
		}

		public override Task OnDisconnectedAsync(Exception exception)
		{
			_connectionIdHolder.RemoveUserInformation(Context.ConnectionId);
			return base.OnDisconnectedAsync(exception);
		}
	}
}