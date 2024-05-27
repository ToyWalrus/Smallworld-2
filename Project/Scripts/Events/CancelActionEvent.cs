namespace Smallworld.Events;

public class CancelActionEvent : IEvent
{
    public string Name => "Cancel action";
    public string CancelledAction { get; } = "";

    public CancelActionEvent() { }

    public CancelActionEvent(string cancelledAction)
    {
        CancelledAction = cancelledAction;
    }
}