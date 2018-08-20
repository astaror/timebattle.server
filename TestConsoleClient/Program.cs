using System;
using System.Threading.Tasks;
using ClientProtocol;
using Entities.Player;
using Entities.World;

namespace TestConsoleClient
{
  class Program
  {
    static async Task Main(string[] args)
    {
	    var amountOfClients = 100;
	    for (int i = 0; i < amountOfClients; i++)
	    {
		    await StartNewPlayer();
	    }
	    Console.ReadLine();
    }

	  static async Task StartNewPlayer()
	  {
		  var connectionManager = new ConnectionManager();
		  connectionManager.SectorHubClient.OnPlayerEnters += id => Console.WriteLine($"Player {id} enters region");
		  connectionManager.SectorHubClient.OnPlayerLeaves += id => Console.WriteLine($"Player {id} leaves region");
		  connectionManager.SectorHubClient.OnPlayerEntersPosition += (id, position) => Console.WriteLine($"Player {id} enters new position");
		  connectionManager.SectorHubClient.OnPlayerLeftPosition += (id, position) => Console.WriteLine($"Player {id} leaves old position");
		  var playerId = Guid.NewGuid();
		  await connectionManager.Initialize(playerId);
		  await connectionManager.PlayerHubClient.Login(playerId);
	  }
  }
}
