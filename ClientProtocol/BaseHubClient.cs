using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientProtocol
{
	public abstract class BaseHubClient : IDisposable
	{
		protected HubConnection Connection;
		protected Boolean IsConnected;

		protected abstract String HubIdentifier();

		protected BaseHubClient(String url)
		{
			var fullUrl = url + "/" + HubIdentifier();
			Connection = new HubConnectionBuilder()
				.WithUrl(fullUrl)
				.Build();
		}

		public virtual async Task Initialize(Guid playerId)
		{
			await Connection.StartAsync();
			IsConnected = true;
			await Connection.InvokeAsync("IdentifyPlayer", playerId);
		}

		public async void Dispose()
		{
			if (Connection != null && IsConnected) await Connection.StopAsync();
		}
	}
}