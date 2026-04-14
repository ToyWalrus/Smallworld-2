using Smallworld.Events;
using Smallworld.Models;

public class RacePowerEnterDeclineHook : IHook
{
	public string Name => "RacePowerEnterDecline";
	public RacePower RacePower;
}