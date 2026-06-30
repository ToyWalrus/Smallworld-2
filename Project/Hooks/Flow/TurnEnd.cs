using Smallworld.Models;

namespace Smallworld.Hooks;

public class TurnEndHook : IHook
{
	public string Name => "TurnEnd";
	public Player Player;
}