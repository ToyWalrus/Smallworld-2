namespace Smallworld.Events;

public interface IEvent
{
    string Name { get; }

    public string ToString() => Name;
}
