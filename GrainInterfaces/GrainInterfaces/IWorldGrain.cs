using System.Threading.Tasks;
using GrainInterfaces.ManagementInterfaces;
using GrainInterfaces.PublicInterfaces;
using Orleans;

namespace GrainInterfaces.GrainInterfaces
{
	public interface IWorldGrain : IGrainWithGuidKey, IWorld, IWorldManagement
	{
		Task LoadWorld();
	}
}