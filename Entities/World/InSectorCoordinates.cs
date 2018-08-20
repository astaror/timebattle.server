using System;

namespace Entities.World
{
	public class InSectorCoordinates
	{
		public Byte X { get; protected set; }
		public Byte Y { get; protected set; }

		public InSectorCoordinates(Byte x, Byte y)
		{
			X = x;
			Y = y;
		}
	}
}