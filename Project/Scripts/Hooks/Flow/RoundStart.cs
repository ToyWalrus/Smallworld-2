namespace Smallworld.Hooks;

public class RoundStartHook : IHook
{
	public string Name => "RoundStart";
	public int RoundNumber = 0;
}