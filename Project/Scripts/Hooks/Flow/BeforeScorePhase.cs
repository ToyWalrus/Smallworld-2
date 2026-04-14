using Smallworld.Models;

namespace Smallworld.Hooks;

public class BeforeScorePhaseHook : IHook
{
	public string Name => "BeforeScorePhase";
	public Player Player;
}