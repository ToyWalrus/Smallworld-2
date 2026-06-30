namespace Smallworld.Hooks;

public interface IHook
{
    string Name { get; }

    public string ToString() => Name;
}
