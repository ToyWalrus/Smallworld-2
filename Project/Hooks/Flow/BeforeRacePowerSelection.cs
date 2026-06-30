using Smallworld.Models;

namespace Smallworld.Hooks;

public class BeforeRacePowerSelectionHook : IHook
{
	public string Name => "BeforeRacePowerSelection";
	public Player Player;
}