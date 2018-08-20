using GrainInterfaces.PublicInterfaces;
using Orleans;

namespace GrainInterfaces.GrainInterfaces
{
	public interface IPlayerGrain : IGrainWithGuidKey, IPlayer
	{
		
	}
}