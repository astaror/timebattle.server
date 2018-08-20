using System;
using System.Collections.Generic;
using Entities.Currency;
using Entities.World;

namespace Entities.Player
{
	public class PlayerProfile
	{
		public String Name { get; set; }
		public Dictionary<CurrencyTypeEnum, UInt32> Currency { get; set; }

		public PlayerProfile()
		{
			Name = String.Empty;
			Currency = new Dictionary<CurrencyTypeEnum, UInt32>();
		}

		#region Player world position

		public SectorCoordinates CurrentSector { get; set; }
		public InSectorCoordinates InSector { get; set; }

		#endregion
	}
}