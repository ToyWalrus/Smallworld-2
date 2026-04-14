using Smallworld.Events;
using Smallworld.Models;

public class BeforeScorePhaseHook : IHook
{
	public string Name => "BeforeScorePhase";
	public Player Player;
}