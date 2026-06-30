using Smallworld.Models;

namespace Smallworld.Hooks;

public class RacePowerEnterDeclineHook : IHook
{
	public string Name => "RacePowerEnterDecline";
	public RacePower RacePower;
}