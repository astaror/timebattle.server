using System;
using Entities.World.BlockObjects;

namespace Entities.World
{
	public class Sector
	{
		public Byte Width { get; set; } 
		public Byte Height { get; set; }
		public SectorThemeEnum SectorTheme { get; set; }
		public BlockObject[,] SectorData { get; set; }
	}
}