namespace Smallworld.Models.Races;

public class Troll : Race
{
    public Troll() : base()
    {
        Name = "Trolls";
        StartingTokenCount = 5;
        MaxTokens = 10;
    }

    public override void OnRegionConquered(Region region)
    {
        base.OnRegionConquered(region);
        region.AddToken(Token.TrollLair);
    }
}
