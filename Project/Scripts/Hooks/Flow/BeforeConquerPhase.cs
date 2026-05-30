using Smallworld.Models;

namespace Smallworld.Hooks;

public class BeforeConquerPhaseHook : IHook
{
	public string Name => "BeforeConquerPhase";
	public Player Player;
}