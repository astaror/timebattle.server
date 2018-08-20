using System;
using System.Threading.Tasks;
using Entities.Player;
using GrainInterfaces;
using GrainInterfaces.GrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace TimebattleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {

	    private readonly IClusterClient _clusterClient;

	    public PlayerController(IClusterClient clusterClient)
	    {
		    _clusterClient = clusterClient;
	    }

	    // GET api/values/5
        [HttpGet("{id}")]
        public async Task<PlayerProfile> Get(Guid id)
        {
	        var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(id);
	        return await playerGrain.GetPlayerProfile();
        }

		[HttpPut("setPlayeName")]
	    public async Task<Boolean> SetPlayerName(Guid id, String name)
	    {
		    var playerGrain = _clusterClient.GetGrain<IPlayerGrain>(id);
		    return await playerGrain.SetPlayerName(name);
	    }
    }
}
