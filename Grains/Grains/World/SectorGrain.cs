using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.World;
using GrainInterfaces.GrainInterfaces;
using GrainInterfaces.Observers;
using Grains.GrainStates;
using Grains.Utility;
using InternalMessages.Sector;
using Orleans;
using Orleans.Providers;

namespace Grains.Grains.World
{
	[StorageProvider(ProviderName = "world")]
	public class SectorGrain : Grain<SectorGrainState>, ISectorGrain
	{
		private readonly IKafkaProducer _kafkaProducer = new KafkaProducer();
		private const String KafkaQueue = "Sector";
		private String _sectorKey;

		public async Task LoadSector(Sector sectorData, SectorCoordinates coordinates)
		{
			State.SectorData = sectorData;
			if (State.PlayerPositions == null) State.PlayerPositions = new PlayerPositionDictionary();
			if (State.PlayersInSector == null) State.PlayersInSector = new List<Guid>();
			State.SectorCoordinates = coordinates;
			_sectorKey = coordinates.ToString();
			await WriteStateAsync();
		}

		public async Task UnloadSector()
		{
			State.SectorData = null;
			await WriteStateAsync();
		}

		/// <summary>
		///     Called when player enters the sector
		/// </summary>
		/// <param name="playerGuid"></param>
		/// <param name="coordinates"></param>
		/// <returns></returns>
		public async Task<(Boolean, InSectorCoordinates)> SpawnPlayer(Guid playerGuid, InSectorCoordinates coordinates)
		{
			if (State.SectorData.Width <= coordinates.X || State.SectorData.Height <= coordinates.Y) return (false, null);
			if (!State.SectorData.SectorData[coordinates.X, coordinates.Y].IsWalkable) return (false, null);
			if (State.PlayersInSector.Contains(playerGuid)) State.PlayersInSector.Remove(playerGuid);
			if (State.PlayerPositions.ContainsKey(playerGuid)) PlayerLeftPosition(playerGuid, State.PlayerPositions[playerGuid]);

			PlayerEntersSector(playerGuid);
			State.PlayersInSector.Add(playerGuid);
			State.PlayerPositions[playerGuid] = coordinates;
			PlayerSetPosition(playerGuid, coordinates);
			await WriteStateAsync();
			return (true, coordinates);
		}

		/// <summary>
		///     Called when player exits the sector
		/// </summary>
		/// <param name="playerGuid"></param>
		/// <returns></returns>
		public async Task DespawnPlayer(Guid playerGuid)
		{
			if (State.PlayerPositions.ContainsKey(playerGuid))
			{
				PlayerLeftPosition(playerGuid, State.PlayerPositions[playerGuid]);
				State.PlayerPositions.Remove(playerGuid);
			}


			if (State.PlayersInSector.Contains(playerGuid))
			{
				PlayerLeftSector(playerGuid);
				State.PlayersInSector.Remove(playerGuid);
			}

			await WriteStateAsync();
		}

		public async Task<(Boolean, InSectorCoordinates)> MovePlayerTo(Guid playerGuid, InSectorCoordinates coordinates)
		{
			if (!State.PlayersInSector.Contains(playerGuid) || !State.PlayerPositions.ContainsKey(playerGuid)) return (false, null);

			var currentPosition = State.PlayerPositions[playerGuid];

			if (State.SectorData.Width <= coordinates.X || State.SectorData.Height <= coordinates.Y) return (false, currentPosition);
			if (!State.SectorData.SectorData[coordinates.X, coordinates.Y].IsWalkable) return (false, currentPosition);
			// For now allow movement only by one non-diagonal cell

			if ((currentPosition.X != coordinates.X || Math.Max(currentPosition.Y, coordinates.Y) - Math.Min(currentPosition.Y, coordinates.Y) != 1) &&
			    (currentPosition.Y != coordinates.Y || Math.Max(currentPosition.X, coordinates.X) - Math.Min(currentPosition.X, coordinates.X) != 1)) return (false, currentPosition);

			PlayerLeftPosition(playerGuid, currentPosition);
			PlayerSetPosition(playerGuid, coordinates);
			State.PlayerPositions[playerGuid] = coordinates;
			await WriteStateAsync();
			return (true, coordinates);
		}

		public Task<InSectorCoordinates> GetSafeCoordinates()
		{
			if (State.SectorData?.SectorData == null) return Task.FromResult<InSectorCoordinates>(null);
			for (Byte x = 0; x < State.SectorData.Width; x++)
			for (Byte y = 0; y < State.SectorData.Height; y++)
				if (State.SectorData.SectorData[x, y].IsWalkable)
					return Task.FromResult(new InSectorCoordinates(x, y));
			return Task.FromResult<InSectorCoordinates>(null);
		}

		/// <summary>
		///     Fires event to observers about player left his previous position in sector
		/// </summary>
		/// <param name="playerId"></param>
		/// <param name="position"></param>
		private void PlayerLeftPosition(Guid playerId, InSectorCoordinates position)
		{
			_kafkaProducer.PushToQueue(KafkaQueue, _sectorKey, new SectorPlayerLeftPosition()
			{
				PlayerId = playerId,
				SectorCoordinates = State.SectorCoordinates,
				Position = position
			});
//			_subscriptionManager.Notify(observer => observer.PlayerLeftPosition(State.SectorCoordinates, playerId, position));
		}

		/// <summary>
		///     Fires event to observers about player set new position in sector
		/// </summary>
		/// <param name="playerId"></param>
		/// <param name="position"></param>
		private void PlayerSetPosition(Guid playerId, InSectorCoordinates position)
		{
			_kafkaProducer.PushToQueue(KafkaQueue, _sectorKey, new SectorPlayerSetPosition()
			{
				PlayerId = playerId,
				SectorCoordinates = State.SectorCoordinates,
				Position = position
			});
//			_subscriptionManager.Notify(observer => observer.PlayerSetPosition(State.SectorCoordinates, playerId, position));
		}

		/// <summary>
		///     Fires event to observers about player entered sector
		/// </summary>
		/// <param name="playerId"></param>
		private void PlayerEntersSector(Guid playerId)
		{
			_kafkaProducer.PushToQueue(KafkaQueue, _sectorKey, new SectorPlayerEnter()
			{
				PlayerId = playerId,
				SectorCoordinates = State.SectorCoordinates,
			});
//			_subscriptionManager.Notify(observer => observer.PlayerEntersSector(State.SectorCoordinates, playerId));
		}

		/// <summary>
		///     Fires event to observers about player left sector
		/// </summary>
		/// <param name="playerId"></param>
		private void PlayerLeftSector(Guid playerId)
		{
			_kafkaProducer.PushToQueue(KafkaQueue, _sectorKey, new SectorPlayerLeft()
			{
				PlayerId = playerId,
				SectorCoordinates = State.SectorCoordinates,
			});
//			_subscriptionManager.Notify(observer => observer.PlayerLeftSector(State.SectorCoordinates, playerId));
		}
	}
}