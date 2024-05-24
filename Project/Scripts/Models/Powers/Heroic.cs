using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
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
            }

            return new() { Token.Heroic, Token.Heroic };
        }
    }
}