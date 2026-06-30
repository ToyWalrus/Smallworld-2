using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Powers;

public class Bivouacking : Power
{
    public Bivouacking()
    {
        Name = "Bivouacking";
        StartingTokenCount = 5;
    }

    public override List<Token> GetRedeploymentTokens(List<Region> ownedRegions)
    {
        foreach (var region in ownedRegions)
        {
            region.RemoveAllTokensOfType(Token.Encampment);
        }

        return Enumerable.Repeat(Token.Encampment, 5).ToList();
    }

    protected override void OnEnterDecline(List<Region> ownedRegions)
    {
        foreach (var region in ownedRegions)
        {
            region.RemoveAllTokensOfType(Token.Encampment);
        }
    }
}
