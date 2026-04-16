using System.Collections.Generic;

namespace Smallworld.Models.Powers;

public class Heroic : Power
{
    public Heroic()
    {
        Name = "Heroic";
        StartingTokenCount = 5;
    }

    public override List<Token> GetRedeploymentTokens(List<Region> ownedRegions)
    {
        foreach (var region in ownedRegions)
        {
            region.RemoveAllTokensOfType(Token.Heroic);
            region.SetImmune(false);
        }

        return new() { Token.Heroic, Token.Heroic };
    }

    public void PlaceHeroicToken(Region region)
    {
        region.AddToken(Token.Heroic);
        region.SetImmune(true);
    }

    protected override void OnEnterDecline(List<Region> ownedRegions)
    {
        foreach (var region in ownedRegions)
        {
            region.RemoveAllTokensOfType(Token.Heroic);
            region.SetImmune(false);
        }
    }
}
