using Smallworld.Events;
using Smallworld.Models;

public class AfterScorePhaseHook : IHook
{
	public string Name => "AfterScorePhase";
	public Player Player;
	public int VPScored;
}