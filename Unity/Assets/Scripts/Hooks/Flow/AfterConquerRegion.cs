using Smallworld.Events;
using Smallworld.Models;

public class AfterConquerRegionHook : IHook
{
	public string Name => "AfterConquerRegion";
	public RacePower RacePower;
	public int FinalConquerCount;
	public Region Region;
}