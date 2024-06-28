namespace Smallworld.Events;

public class UIInteractionEvent : IEvent
{
    public enum Types
    {
        Confirm,
        Cancel,
        RollDice,
        EndTurn,
        EnterDecline,
        NewPlayer
    }

    public string Name => ToString();
    public Types InteractionType { get; }

    public UIInteractionEvent(Types interactionType)
    {
        InteractionType = interactionType;
    }

    public override string ToString()
    {
        var str = "UI Interaction";

        switch (InteractionType)
        {
            case Types.Confirm:
                str += " (Confirm)";
                break;
            case Types.Cancel:
                str += " (Cancel)";
                break;
            case Types.RollDice:
                str += " (Roll dice)";
                break;
            case Types.EndTurn:
                str += " (End turn)";
                break;
            case Types.NewPlayer:
                str += " (New player)";
                break;
            case Types.EnterDecline:
                str += " (Enter decline)";
                break;
        }

        return str;
    }

    public static readonly UIInteractionEvent Confirm = new(Types.Confirm);
    public static readonly UIInteractionEvent Cancel = new(Types.Cancel);
    public static readonly UIInteractionEvent RollDice = new(Types.RollDice);
    public static readonly UIInteractionEvent EndTurn = new(Types.EndTurn);
    public static readonly UIInteractionEvent EnterDecline = new(Types.EnterDecline);
}