using Smallworld.Events;
using Smallworld.Models;

public class TurnStartHook : IHook
{
	public string Name => "TurnStart";
	public Player Player;
}