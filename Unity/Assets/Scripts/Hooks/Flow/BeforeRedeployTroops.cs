using Smallworld.Events;
using Smallworld.Models;

public class BeforeRedeployTroopsHook : IHook
{
	public string Name => "BeforeRedeployTroops";
	public RacePower RacePower;
	public int DeployCount;
	public Region Region;
}