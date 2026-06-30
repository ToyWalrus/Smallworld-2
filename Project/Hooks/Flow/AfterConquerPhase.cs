using Smallworld.Models;

namespace Smallworld.Hooks;

public class AfterConquerPhaseHook : IHook
{
    public string Name => "AfterConquerPhase";
    public Player Player;
}