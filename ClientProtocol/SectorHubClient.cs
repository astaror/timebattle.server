using System;
using System.Threading.Tasks;
using ConnectionInterfaces;
using Entities.World;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientProtocol
{
	public class SectorHubClient : BaseHubClient, ISectorHub
	{
		public delegate void PlayerSectorAssociation(Guid playerId);

		public delegate void SectorDataChanged(Sector sector);

		public SectorHubClient(String url) : base(url)
		{
		}

		public override async Task Initialize(Guid playerId)
		{
			Connection.On<Sector>("SectorChanged", sector => SectorChanged(sector));
			Connection.On<Guid>("PlayerEnterSector", _playerId => PlayerEnterSector(_playerId));
			Connection.On<Guid>("PlayerLeftSector", _playerId => PlayerLeftSector(_playerId));
			Connection.On<Guid, InSectorCoordinates>("PlayerLeftPosition", (_playerId, position) => PlayerLeftPosition(_playerId, position));
			Connection.On<Guid, InSectorCoordinates>("PlayerEntersPosition", (_playerId, position) => PlayerEntersPosition(_playerId, position));
			await base.Initialize(playerId);
		}

		public Task SectorChanged(Sector sector)
		{
			OnSectorDataChanged?.Invoke(sector);
			return Task.CompletedTask;
		}

		public Task PlayerEnterSector(Guid playerId)
		{
			OnPlayerEnters?.Invoke(playerId);
			return Task.CompletedTask;
		}

		public Task PlayerLeftSector(Guid playerId)
		{
			OnPlayerLeaves?.Invoke(playerId);
			return Task.CompletedTask;
		}

		protected override String HubIdentifier()
		{
			return "SectorHub";
		}

		public event SectorDataChanged OnSectorDataChanged;

		public event PlayerSectorAssociation OnPlayerEnters;
		public event PlayerSectorAssociation OnPlayerLeaves;

		#region Players position handlers

		public delegate void PlayerChangedPosition(Guid playerId, InSectorCoordinates position);

		public event PlayerChangedPosition OnPlayerLeftPosition;
		public event PlayerChangedPosition OnPlayerEntersPosition;

		public Task PlayerLeftPosition(Guid playerId, InSectorCoordinates position)
		{
			OnPlayerLeftPosition?.Invoke(playerId, position);
			return Task.CompletedTask;
		}

		public Task PlayerEntersPosition(Guid playerId, InSectorCoordinates position)
		{
			OnPlayerEntersPosition?.Invoke(playerId, position);
			return Task.CompletedTask;
		}

		#endregion
	}
}