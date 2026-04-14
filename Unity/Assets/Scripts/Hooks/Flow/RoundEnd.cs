using Smallworld.Events;

public class RoundEndHook : IHook
{
	public string Name => "RoundEnd";
	public int RoundNumber = 0;
}