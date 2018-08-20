using System;

namespace TimebattleApi.Services.Sector
{
	public interface IConnectionIdHolder
	{
		Guid GetUserIdByConnectionId(String connectionId);
		String GetConnectionIdByUserId(Guid userId);
		void AddUserInformation(Guid userId, String connectionId);
		void RemoveUserInformation(String connectionId);
	}
}