using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GrainInterfaces.GrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Grains.Utility;

namespace GrainsSilo
{
  class Program
    {
        static async Task Main(string[] args)
        {
	        try
	        {
	            Console.WriteLine("Starting silo...");
		        var silo = await StartSilo();
		        Console.WriteLine("Silo up");
		        var ipaddresses = Dns.GetHostEntry(Environment.GetEnvironmentVariable("SELF")).AddressList;
		        foreach (var ipaddress in ipaddresses)
		        {
			        Console.WriteLine($"IP ADDRESS: {ipaddress}");
		        }
		        Thread.Sleep(Timeout.Infinite);
		        await silo.StopAsync();
	        }
	        catch (Exception ex)
	        {
		        Console.WriteLine(ex);
		        Console.ReadLine();
	        }

        }

	    private static bool _IsPrivate(string ipAddress)
	    {
		    int[] ipParts = ipAddress.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries)
			    .Select(s => int.Parse(s)).ToArray();
		    // in private ip range
		    if (ipParts[0] == 10 ||
		        (ipParts[0] == 192 && ipParts[1] == 168) ||
		        (ipParts[0] == 172 && (ipParts[1] >= 16 && ipParts[1] <= 31))) {
			    return true;
		    }

		    // IP Address is probably public.
		    // This doesn't catch some VPN ranges like OpenVPN and Hamachi.
		    return false;
	    }

	    private static async Task<ISiloHost> StartSilo()
	    {
		    var builder = new SiloHostBuilder()
			    // Use localhost clustering for a single local silo
			    .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000, advertisedIP: Dns.GetHostEntry(Environment.GetEnvironmentVariable("SELF")).AddressList[0])
			    // Configure ClusterId and ServiceId
			    .Configure<ClusterOptions>(options =>
			    {
				    options.ClusterId = "timebattle.server";
				    options.ServiceId = "TimebattleService";
			    })
			    // Configure logging with any logging framework that supports Microsoft.Extensions.Logging.
			    // In this particular case it logs using the Microsoft.Extensions.Logging.Console package.
			    .ConfigureLogging(logging => logging.AddConsole())
			    .ConfigureServices(svc => svc.AddSingleton<IKafkaProducer>(new KafkaProducer()));
		    var mainConnectionString = Environment.GetEnvironmentVariable("CLUSTER_CONNECTION_STRING") ?? "localhost";
		    var playerDataConnectionString = Environment.GetEnvironmentVariable("PLAYER_CONNECTION_STRING") ?? "localhost";
		    var worldConnectionString = Environment.GetEnvironmentVariable("WORLD_CONNECTION_STRING") ?? "localhost";
		    builder.AddMongoDBGrainStorage("player", _ =>
		    {
			    _.ConnectionString = playerDataConnectionString;
			    _.DatabaseName = "Players";
		    });
		    builder.AddMongoDBGrainStorage("world", _ =>
		    {
			    _.ConnectionString = worldConnectionString;
			    _.DatabaseName = "World";
		    });
		    builder.UseMongoDBClustering(_ =>
		    {
			    _.ConnectionString = mainConnectionString;
			    _.DatabaseName = "Timebattle";
		    });
		    builder.UseMongoDBReminders(options =>
		    {
			    options.ConnectionString = mainConnectionString;
			    options.DatabaseName = "Timebattle";
		    });
		    builder.AddStartupTask(async (s, ct) =>
		    {
			    var grainFactory = s.GetRequiredService<IGrainFactory>();
			    await grainFactory.GetGrain<IWorldGrain>(Guid.Empty).LoadWorld();
		    });

		    var host = builder.Build();
		    await host.StartAsync();
			
		    return host;
	    }
    }
}
