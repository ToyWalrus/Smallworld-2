namespace Smallworld.Models.Races;

public class Ghoul : Race
{
    public Ghoul() : base()
    {
        Name = "Ghouls";
        StartingTokenCount = 5;
        MaxTokens = 10;
    }

    public override void EnterDecline()
    {
        IsInDecline = false;
    }
}
