using System;
using Entities.World;
using Entities.World.BlockObjects;

namespace ManagementApi.WorldManagement
{
	public static class SectorGenerator
	{
		public static Sector GeneratePlainSector(Byte width, Byte height)
		{
			var sector = new Sector
			{
				Width = width,
				Height = height,
				SectorTheme = SectorThemeEnum.Nature,
				SectorData = new BlockObject[width, height]
			};
			for (var x = 0; x < width; x++)
			for (var y = 0; y < height; y++)
				sector.SectorData[x, y] = new PlainBlockObject();
			return sector;
		}

		public static Sector GenerateBoundedSector(Byte width, Byte height)
		{
			var sector = new Sector
			{
				Width = width,
				Height = height,
				SectorTheme = SectorThemeEnum.Nature,
				SectorData = new BlockObject[width, height]
			};
			for (var x = 0; x < width; x++)
			for (var y = 0; y < height; y++)
				if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
					sector.SectorData[x, y] = new BlockerBlockObject(1);
				else
					sector.SectorData[x, y] = new PlainBlockObject();
			return sector;
		}
	}
}