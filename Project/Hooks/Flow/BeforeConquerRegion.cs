using Smallworld.Models;

namespace Smallworld.Hooks;

public class BeforeConquerRegionHook : IHook
{
	public string Name => "BeforeConquerRegion";
	public RacePower RacePower;
	public int ConquerCount;
	public Region Region;
}