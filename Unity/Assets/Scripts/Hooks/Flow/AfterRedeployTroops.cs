using Smallworld.Events;
using Smallworld.Models;

public class AfterRedeployTroopsHook : IHook
{
	public string Name => "AfterRedeployTroops";
	public RacePower RacePower;
	public int FinalDeployCount;
	public Region Region;
}