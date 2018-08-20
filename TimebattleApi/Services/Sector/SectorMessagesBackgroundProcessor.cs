using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using ConnectionInterfaces;
using Entities.World;
using GrainInterfaces.GrainInterfaces;
using GrainInterfaces.Observers;
using InternalMessages.Sector;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Orleans;
using TimebattleApi.Hubs;

namespace TimebattleApi.Services.Sector
{
	public class SectorMessagesBackgroundProcessor : BackgroundService, ISectorObserver
	{
		private readonly IConnectionIdHolder _connectionIdHolder;
		private readonly IClusterClient _clusterClient;
		private readonly IHubContext<SectorHub, ISectorHub> _sectorHubContext;
		private readonly Consumer<Ignore, String> consumer;

		public SectorMessagesBackgroundProcessor(IClusterClient clusterClient, IConnectionIdHolder connectionIdHolder, IHubContext<SectorHub, ISectorHub> sectorHubContext)
		{
			_clusterClient = clusterClient;
			_connectionIdHolder = connectionIdHolder;
			_sectorHubContext = sectorHubContext;

			// Initialize Kafka consumer

			var conf = new Dictionary<String, Object> 
			{ 
				{ "group.id", "sector-consumer-group" },
				{ "bootstrap.servers", Environment.GetEnvironmentVariable("KAFKA") },
				{ "auto.commit.interval.ms", 5000 },
				{ "auto.offset.reset", "earliest" }
			};

			consumer = new Consumer<Ignore, String>(conf, new IgnoreDeserializer(), new StringDeserializer(Encoding.UTF8));
			consumer.OnError += (_, error)
				=> Console.WriteLine($"Error: {error}");

			// Raised when the consumer is assigned a new set of partitions.
			consumer.OnPartitionAssignmentReceived += (_, partitions) =>
			{
				Console.WriteLine($"Assigned partitions: [{String.Join(", ", partitions)}], member id: {consumer.MemberId}");
				// If you don't add a handler to the OnPartitionsAssigned event,
				// the below .Assign call happens automatically. If you do, you
				// must call .Assign explicitly in order for the consumer to 
				// start consuming messages.
				consumer.Assign(partitions);
			};

			// Raised when the consumer's current assignment set has been revoked.
			consumer.OnPartitionAssignmentRevoked += (_, partitions) =>
			{
				Console.WriteLine($"Revoked partitions: [{String.Join(", ", partitions)}]");
				// If you don't add a handler to the OnPartitionsRevoked event,
				// the below .Unassign call happens automatically. If you do, 
				// you must call .Unassign explicitly in order for the consumer
				// to stop consuming messages from it's previously assigned 
				// partitions.
				consumer.Unassign();
			};

			consumer.OnStatistics += (_, json)
				=> Console.WriteLine($"Statistics: {json}");
			consumer.Subscribe("Sector");
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (true && !stoppingToken.IsCancellationRequested)
			{
				try
				{
					var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(5));
					if (consumeResult.IsPartitionEOF)
					{
						//Console.WriteLine($"Reached end of topic {consumeResult.Topic} partition {consumeResult.Partition}, next message will be at offset {consumeResult.Offset}");
					}
					if (consumeResult.Message == null)
					{
						continue;
					}

					Console.WriteLine($"Topic: {consumeResult.Topic} Partition: {consumeResult.Partition} Offset: {consumeResult.Offset} {consumeResult.Value}");
					var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
					var parsedMessage = JsonConvert.DeserializeObject<BaseSectorMessage>(consumeResult.Value, settings);
					if (parsedMessage == null)
					{
						Console.WriteLine($"Can't deserialize message {consumeResult.Value}");
						continue;
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

					if (consumeResult.Offset % 5 == 0)
					{
						var committedOffsets = consumer.CommitAsync(consumeResult, stoppingToken).Result;
						//Console.WriteLine($"Committed offset: {committedOffsets}");
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"Consume error: {e.Message}");
				}
			}
		}

		public async Task PlayerLeftPosition(SectorCoordinates coordinates, Guid playerId, InSectorCoordinates position)
		{
			Console.WriteLine("1");
			var groupName = coordinates.ToString();
			Console.WriteLine("2");
			await _sectorHubContext.Clients.Group(groupName).PlayerLeftPosition(playerId, position);
			Console.WriteLine("3");
		}

		public async Task PlayerSetPosition(SectorCoordinates coordinates, Guid playerId, InSectorCoordinates position)
		{
			Console.WriteLine("1");
			var groupName = coordinates.ToString();
			Console.WriteLine("2");
			await _sectorHubContext.Clients.Group(groupName).PlayerEntersPosition(playerId, position);
			Console.WriteLine("3");
		}

		public async Task PlayerEntersSector(SectorCoordinates coordinates, Guid playerId)
		{
			Console.WriteLine("1");
			var groupName = coordinates.ToString();
			Console.WriteLine("2");
			var connectionIdByUserId = _connectionIdHolder.GetConnectionIdByUserId(playerId);
			await _sectorHubContext.Groups.AddToGroupAsync(connectionIdByUserId, groupName);
			Console.WriteLine("3");
			var world = _clusterClient.GetGrain<IWorldGrain>(Guid.Empty);
			Console.WriteLine("4");
			var sectorData = world.GetSector(coordinates).Result;
			Console.WriteLine("5");
			await _sectorHubContext.Clients.Client(connectionIdByUserId).SectorChanged(sectorData);
			Console.WriteLine("6");
			await _sectorHubContext.Clients.Group(groupName).PlayerEnterSector(playerId);
			Console.WriteLine("7");
		}

		public async Task PlayerLeftSector(SectorCoordinates coordinates, Guid playerId)
		{
			Console.WriteLine("1");
			var groupName = coordinates.ToString();
			await _sectorHubContext.Groups.RemoveFromGroupAsync(_connectionIdHolder.GetConnectionIdByUserId(playerId), groupName);
			Console.WriteLine("2");
			await _sectorHubContext.Clients.Group(groupName).PlayerLeftSector(playerId);
			Console.WriteLine("3");
			Console.WriteLine("4");
		}
	}
}