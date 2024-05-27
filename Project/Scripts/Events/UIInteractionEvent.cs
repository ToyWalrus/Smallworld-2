namespace Smallworld.Events;

public class UIInteractionEvent : IEvent
{
    public enum Types
    {
        Confirm,
        RollDice,
        EndTurn,
        NewPlayer
    }

    public string Name => "UI interaction";
    public Types InteractionType { get; }

    public UIInteractionEvent(Types interactionType)
    {
        InteractionType = interactionType;
    }

    public override string ToString()
    {
        var str = Name;

        switch (InteractionType)
        {
            case Types.Confirm:
                str += " (Confirm)";
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
        }

        return str;
    }
}