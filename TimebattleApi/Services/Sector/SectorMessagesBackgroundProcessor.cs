using System;
using System.Threading;
using System.Threading.Tasks;
using ConnectionInterfaces;
using Entities.World;
using GrainInterfaces.GrainInterfaces;
using GrainInterfaces.Observers;
using InternalMessages.Sector;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Orleans;
using StackExchange.Redis;
using TimebattleApi.Hubs;

namespace TimebattleApi.Services.Sector
{
  public class SectorMessagesBackgroundProcessor : BackgroundService, ISectorObserver
	{
		private readonly IConnectionIdHolder _connectionIdHolder;
		private readonly IClusterClient _clusterClient;
		private readonly IHubContext<SectorHub, ISectorHub> _sectorHubContext;
		private ISubscriber consumer;
		private readonly JsonSerializerSettings settings;

		public SectorMessagesBackgroundProcessor(IClusterClient clusterClient, IConnectionIdHolder connectionIdHolder, IHubContext<SectorHub, ISectorHub> sectorHubContext)
		{
			_clusterClient = clusterClient;
			_connectionIdHolder = connectionIdHolder;
			_sectorHubContext = sectorHubContext;
			settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			var redisServer = Environment.GetEnvironmentVariable("REDIS");
			var redis       = ConnectionMultiplexer.Connect(redisServer);
			consumer = redis.GetSubscriber();
			await consumer.SubscribeAsync("Sector", async (channel, message) => await Handler(channel, message));
			while (true && !stoppingToken.IsCancellationRequested)
			{
				await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
			}
		}

		private async Task Handler(RedisChannel arg1, RedisValue arg2)
		{
			var parsedMessage = JsonConvert.DeserializeObject<BaseSectorMessage>(arg2, settings);
			if (parsedMessage == null)
			{
				Console.WriteLine($"Can't deserialize message {arg2}");
				return;
			}
			switch (parsedMessage)
			{
				case SectorPlayerLeft msg:
					Console.WriteLine("SectorPlayerLeft received");
					await PlayerLeftSector(msg.SectorCoordinates, msg.PlayerId);
					break;
				case SectorPlayerEnter msg:
					Console.WriteLine("SectorPlayerEnter received");
					await PlayerEntersSector(msg.SectorCoordinates, msg.PlayerId);
					break;
				case SectorPlayerLeftPosition msg:
					Console.WriteLine("SectorPlayerLeftPosition received");
					await PlayerLeftPosition(msg.SectorCoordinates, msg.PlayerId, msg.Position);
					break;
				case SectorPlayerSetPosition msg:
					Console.WriteLine("SectorPlayerSetPosition received");
					await PlayerSetPosition(msg.SectorCoordinates, msg.PlayerId, msg.Position);
					break;
			}
			Console.WriteLine("Message processing finished");
		}

		public async Task PlayerLeftPosition(SectorCoordinates coordinates, Guid playerId, InSectorCoordinates position)
		{
			var groupName = coordinates.ToString();
			await _sectorHubContext.Clients.Group(groupName).PlayerLeftPosition(playerId, position);
		}

		public async Task PlayerSetPosition(SectorCoordinates coordinates, Guid playerId, InSectorCoordinates position)
		{
			var groupName = coordinates.ToString();
			await _sectorHubContext.Clients.Group(groupName).PlayerEntersPosition(playerId, position);
		}

		public async Task PlayerEntersSector(SectorCoordinates coordinates, Guid playerId)
		{
			var groupName = coordinates.ToString();
			var connectionIdByUserId = _connectionIdHolder.GetConnectionIdByUserId(playerId);
			await _sectorHubContext.Groups.AddToGroupAsync(connectionIdByUserId, groupName);
			var world = _clusterClient.GetGrain<IWorldGrain>(Guid.Empty);
			var sectorData = world.GetSector(coordinates).Result;
			await _sectorHubContext.Clients.Client(connectionIdByUserId).SectorChanged(sectorData);
			await _sectorHubContext.Clients.Group(groupName).PlayerEnterSector(playerId);
		}

		public async Task PlayerLeftSector(SectorCoordinates coordinates, Guid playerId)
		{
			var groupName = coordinates.ToString();
			await _sectorHubContext.Groups.RemoveFromGroupAsync(_connectionIdHolder.GetConnectionIdByUserId(playerId), groupName);
			await _sectorHubContext.Clients.Group(groupName).PlayerLeftSector(playerId);
		}
	}
}