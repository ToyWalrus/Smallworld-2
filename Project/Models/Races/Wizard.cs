using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Races;

public class Wizard : Race
{
    public Wizard() : base()
    {
        Name = "Wizards";
        StartingTokenCount = 5;
        MaxTokens = 10;
    }

    public override int TallyRaceBonusVP(List<Region> regions)
    {
        if (IsInDecline) return 0;
        return regions.Count(region => region.HasAttribute(RegionAttribute.Magic));
    }
}
