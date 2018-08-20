using System;
using System.Collections.Concurrent;

namespace TimebattleApi.Services.Sector
{
	public class SectorInMemoryConnectionIdHolder : IConnectionIdHolder
	{
		private readonly ConcurrentDictionary<String, Guid> _connectionIdToUserId;
		private readonly ConcurrentDictionary<Guid, String> _userIdToConnectionId;

		public SectorInMemoryConnectionIdHolder()
		{
			_connectionIdToUserId = new ConcurrentDictionary<String, Guid>();
			_userIdToConnectionId = new ConcurrentDictionary<Guid, String>();
		}

		public Guid GetUserIdByConnectionId(String connectionId)
		{
			return _connectionIdToUserId.TryGetValue(connectionId, out var userId) ? userId : Guid.Empty;
		}

		public String GetConnectionIdByUserId(Guid userId)
		{
			return _userIdToConnectionId.TryGetValue(userId, out var connectionId) ? connectionId : null;
		}

		public void AddUserInformation(Guid userId, String connectionId)
		{
			if (_userIdToConnectionId.ContainsKey(userId)) _userIdToConnectionId.TryRemove(userId, out _);
			if (_connectionIdToUserId.ContainsKey(connectionId)) _connectionIdToUserId.TryRemove(connectionId, out _);
		}

		public void RemoveUserInformation(String connectionId)
		{
			var userId = GetUserIdByConnectionId(connectionId);
			if (_userIdToConnectionId.ContainsKey(userId)) _userIdToConnectionId.TryRemove(userId, out _);
			if (_connectionIdToUserId.ContainsKey(connectionId)) _connectionIdToUserId.TryRemove(connectionId, out _);
		}
	}
}