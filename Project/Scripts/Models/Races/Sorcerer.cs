namespace Smallworld.Models.Races;

public class Sorcerer : Race
{
    public Sorcerer() : base()
    {
        Name = "Sorcerers";
        StartingTokenCount = 5;
        MaxTokens = 18;
    }

    // TODO: prompt player to select a region to place a sorcerer token
}
