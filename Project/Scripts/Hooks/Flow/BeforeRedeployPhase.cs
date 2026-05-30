using Smallworld.Models;

namespace Smallworld.Hooks;

public class BeforeRedeployPhaseHook : IHook
{
    public string Name => "BeforeRedeployPhase";
    public Player Player;
    public int RedployableCount;
}