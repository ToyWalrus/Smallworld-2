using Smallworld.Models;

namespace Smallworld.Hooks;

public class BeforeRedeployTroopsHook : IHook
{
	public string Name => "BeforeRedeployTroops";
	public RacePower RacePower;
}