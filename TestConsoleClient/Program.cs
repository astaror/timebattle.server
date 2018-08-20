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
	    var connectionManager = new ConnectionManager();
	    InitializeEventHandlers(connectionManager);
	    var playerId = Guid.Parse("1CB00EF7-1DA0-4A98-9848-D05F7A99C546");
	    await connectionManager.Initialize(playerId);
	    await connectionManager.PlayerHubClient.Login(playerId);
	    Console.ReadLine();
    }

	  private static void InitializeEventHandlers(ConnectionManager connectionManager)
	  {
		  connectionManager.PlayerHubClient.OnPlayerDataLoaded += PlayerHubClientOnOnPlayerDataLoaded;
		  connectionManager.SectorHubClient.OnSectorDataChanged += SectorHubClientOnOnSectorDataChanged;
		  connectionManager.SectorHubClient.OnPlayerEnters += SectorHubClientOnOnPlayerEnters;
		  connectionManager.SectorHubClient.OnPlayerLeaves += SectorHubClientOnOnPlayerLeaves;
		  connectionManager.SectorHubClient.OnPlayerEntersPosition += SectorHubClientOnOnPlayerEntersPosition;
		  connectionManager.SectorHubClient.OnPlayerLeftPosition += SectorHubClientOnOnPlayerLeftPosition;
	  }

	  private static void SectorHubClientOnOnPlayerLeftPosition(Guid playerid, InSectorCoordinates position)
	  {
		  Console.WriteLine("Player leaves position");
	  }

	  private static void SectorHubClientOnOnPlayerEntersPosition(Guid playerid, InSectorCoordinates position)
	  {
		  Console.WriteLine("Player enters new position");
	  }

	  private static void SectorHubClientOnOnPlayerLeaves(Guid playerid)
	  {
		  Console.WriteLine("Player leaves sector");
	  }

	  private static void SectorHubClientOnOnPlayerEnters(Guid playerid)
	  {
		  Console.WriteLine("Player enters sector");
	  }

	  private static void SectorHubClientOnOnSectorDataChanged(Sector sector)
	  {
		  Console.WriteLine("Sector data loaded");
	  }

	  private static void PlayerHubClientOnOnPlayerDataLoaded(PlayerProfile playerprofile)
	  {
		  Console.WriteLine("Player data loaded");
	  }
  }
}
