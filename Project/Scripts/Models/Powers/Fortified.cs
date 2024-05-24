using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Powers
{
    public class Fortified : Power
    {
        private const int MAX_FORTS = 6;

        public Fortified()
        {
            Name = "Fortified";
            StartingTokenCount = 3;
        }

        public override List<Token> GetRedeploymentTokens(List<Region> ownedRegions)
        {
            if (ownedRegions.Count(region => region.HasToken(Token.Fortress)) >= MAX_FORTS) return new();

            var regionsWithNoFort = ownedRegions.Where(region => !region.HasToken(Token.Fortress));
            if (!regionsWithNoFort.Any()) return new();

            // TODO: prompt player which region to place fort in

            return new() { Token.Fortress };
        }

        public override int TallyPowerBonusVP(List<Region> ownedRegions)
        {
            if (IsInDecline) return 0;
            return ownedRegions.Count(region => region.HasToken(Token.Fortress));
        }
    }
}