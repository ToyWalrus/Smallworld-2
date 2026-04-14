using Smallworld.Events;
using Smallworld.Models;

public class ConquerPhaseStartHook : IHook
{
	public string Name => "ConquerPhaseStart";
	public Player Player;
}