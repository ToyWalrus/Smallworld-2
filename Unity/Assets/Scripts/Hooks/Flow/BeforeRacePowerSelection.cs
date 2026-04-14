using Smallworld.Events;
using Smallworld.Models;

public class BeforeRacePowerSelectionHook : IHook
{
	public string Name => "BeforeRacePowerSelection";
	public Player Player;
}