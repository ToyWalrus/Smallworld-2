using Smallworld.Models;

namespace Smallworld.Hooks;

public class ConquerPhaseStartHook : IHook
{
	public string Name => "ConquerPhaseStart";
	public Player Player;
}