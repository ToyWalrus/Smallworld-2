using Smallworld.Models;

namespace Smallworld.Hooks;

public class AfterConquerRegionHook : IHook
{
	public string Name => "AfterConquerRegion";
	public RacePower RacePower;
	public int FinalConquerCount;
	public Region Region;
}