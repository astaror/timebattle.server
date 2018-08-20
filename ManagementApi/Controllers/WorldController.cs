using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.World;
using GrainInterfaces.GrainInterfaces;
using ManagementApi.WorldManagement;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace ManagementApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WorldController : ControllerBase
	{
		private readonly IClusterClient _clusterClient;

		public WorldController(IClusterClient clusterClient)
		{
			_clusterClient = clusterClient;
		}

		[HttpGet("GetAllSectors")]
		public async Task<ActionResult<Dictionary<SectorCoordinates, Sector>>> GetAllSectors()
		{
			var worldGrain = _clusterClient.GetGrain<IWorldGrain>(Guid.Empty);
			return await worldGrain.GetAllSectors();
		}

		[HttpGet("GetSector")]
		public async Task<ActionResult<Sector>> GetSector([FromQuery] SectorCoordinates coordinates)
		{
			var worldGrain = _clusterClient.GetGrain<IWorldGrain>(Guid.Empty);
			var sector     = await worldGrain.GetSector(coordinates);
			if (sector == null) return NotFound();
			return sector;
		}

		// POST api/values
		[HttpPost("CreateSector")]
		public async Task<ActionResult> CreateSector([FromQuery] SectorCoordinates coordinates, [FromBody] Sector value)
		{
			var worldGrain = _clusterClient.GetGrain<IWorldGrain>(Guid.Empty);
			var result     = await worldGrain.CreateSector(coordinates, value);
			return result ? (ActionResult) Ok() : BadRequest();
		}

		// PUT api/values/5
		[HttpPut("UpdateSector")]
		public async Task<ActionResult> UpdateSector([FromQuery] SectorCoordinates coordinates, [FromBody] Sector value)
		{
			var worldGrain = _clusterClient.GetGrain<IWorldGrain>(Guid.Empty);
			var result     = await worldGrain.UpdateSector(coordinates, value);
			return result ? (ActionResult) Ok() : BadRequest();
		}

		// DELETE api/values/5
		[HttpDelete("DeleteSector")]
		public async Task<ActionResult> DeleteSector([FromQuery] SectorCoordinates coordinates)
		{
			var worldGrain = _clusterClient.GetGrain<IWorldGrain>(Guid.Empty);
			var result     = await worldGrain.DeleteSector(coordinates);
			return result ? (ActionResult) Ok() : BadRequest();
		}

		[HttpPost("GeneratePlainSector")]
		public async Task<ActionResult> GeneratePlainSector(Byte width, Byte height, [FromQuery] SectorCoordinates coordinates)
		{
			var worldGrain = _clusterClient.GetGrain<IWorldGrain>(Guid.Empty);
			var sector     = SectorGenerator.GeneratePlainSector(width, height);
			var result     = await worldGrain.CreateSector(coordinates, sector);
			return result ? (ActionResult) Ok() : BadRequest();
		}

		[HttpPost("GenerateBoundedSector")]
		public async Task<ActionResult> GenerateBoundedSector(Byte width, Byte height, [FromQuery] SectorCoordinates coordinates)
		{
			var worldGrain = _clusterClient.GetGrain<IWorldGrain>(Guid.Empty);
			var sector     = SectorGenerator.GenerateBoundedSector(width, height);
			var result     = await worldGrain.CreateSector(coordinates, sector);
			return result ? (ActionResult) Ok() : BadRequest();
		}

	}
}