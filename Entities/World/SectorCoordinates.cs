using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Entities.World
{
	[TypeConverter(typeof(SectorCoordinatesConverter))]
	public class SectorCoordinates
	{
		public SectorCoordinates(Int32 x, Int32 y, Int32 z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public SectorCoordinates()
		{
		}

		public Int32 X { get; protected set; }
		public Int32 Y { get; protected set; }
		public Int32 Z { get; protected set; }

		public override String ToString()
		{
			return $"{X}:{Y}:{Z}";
		}

		protected Boolean Equals(SectorCoordinates other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public override Boolean Equals(Object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((SectorCoordinates) obj);
		}

		public override Int32 GetHashCode()
		{
			unchecked
			{
				var hashCode = X;
				hashCode = (hashCode * 397) ^ Y;
				hashCode = (hashCode * 397) ^ Z;
				return hashCode;
			}
		}
	}

	internal class SectorCoordinatesConverter : TypeConverter
	{
		public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(String) || base.CanConvertFrom(context, sourceType);
		}

		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
		{
			var data = ((String) value).Split(':').Select(Int32.Parse).ToArray();
			return new SectorCoordinates(data[0], data[1], data[2]);
		}
	}
}