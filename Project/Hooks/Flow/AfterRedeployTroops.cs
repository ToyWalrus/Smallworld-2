using Smallworld.Models;

namespace Smallworld.Hooks;

public class AfterRedeployTroopsHook : IHook
{
	public string Name => "AfterRedeployTroops";
	public RacePower RacePower;
	public int FinalDeployCount;
	public Region Region;
}