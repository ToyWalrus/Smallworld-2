using Smallworld.Models;

namespace Smallworld.Hooks;

public class AfterRedeployPhaseHook : IHook
{
    public string Name => "AfterRedeployPhase";
    public Player Player;
}