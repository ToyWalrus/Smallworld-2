using Smallworld.Events;
using Smallworld.Models;

public class AfterRacePowerSelectionHook : IHook
{
	public string Name => "AfterRacePowerSelection";
	public Player Player;
	public RacePower Selected;
}