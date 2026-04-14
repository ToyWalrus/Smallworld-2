using Smallworld.Models;

namespace Smallworld.Hooks;

public class TurnStartHook : IHook
{
	public string Name => "TurnStart";
	public Player Player;
}