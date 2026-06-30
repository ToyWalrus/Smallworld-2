using Smallworld.Models;

namespace Smallworld.Hooks;

public class AfterScorePhaseHook : IHook
{
	public string Name => "AfterScorePhase";
	public Player Player;
	public int VPScored;
}