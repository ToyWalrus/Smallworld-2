using Smallworld.Models;

namespace Smallworld.Hooks;

public class AfterRacePowerSelectionHook : IHook
{
	public string Name => "AfterRacePowerSelection";
	public Player Player;
	public RacePower Selected;
}