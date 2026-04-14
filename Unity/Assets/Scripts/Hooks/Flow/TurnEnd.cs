using Smallworld.Events;
using Smallworld.Models;

public class TurnEndHook : IHook
{
	public string Name => "TurnEnd";
	public Player Player;
}