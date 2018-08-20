using System;

namespace Entities.World.BlockObjects
{
	public abstract class BlockObject
	{
		public BlockObjectEnum BlockObjectType { get; protected set; }
		public Boolean IsWalkable { get; protected set; }
	}

	public class EmptyBlockObject : BlockObject
	{
		public EmptyBlockObject()
		{
			BlockObjectType = BlockObjectEnum.Empty;
			IsWalkable = false;
		}
	}

	public class PlainBlockObject : BlockObject
	{
		public PlainBlockObject()
		{
			BlockObjectType = BlockObjectEnum.Plain;
			IsWalkable = true;
		}
	}

	public class BlockerBlockObject : BlockObject
	{
		public UInt32 BlockerId { get; protected set; }

		public BlockerBlockObject(UInt32 blockerId)
		{
			BlockObjectType = BlockObjectEnum.Blocker;
			BlockerId = blockerId;
			IsWalkable = false;
		}
	}

	public class SectorConnectionBlockObject : BlockObject
	{
		public SectorCoordinates TargetSector { get; protected set; }
		public InSectorCoordinates InSectorCoordinates { get; protected set; }

		public SectorConnectionBlockObject(SectorCoordinates targetSector, InSectorCoordinates inSectorCoordinates)
		{
			BlockObjectType = BlockObjectEnum.SectorConnection;
			TargetSector = targetSector;
			InSectorCoordinates = inSectorCoordinates;
			IsWalkable = true;
		}
	}
}