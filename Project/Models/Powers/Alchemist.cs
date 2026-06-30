using System.Collections.Generic;

namespace Smallworld.Models.Powers;

public class Alchemist : Power
{
    public Alchemist()
    {
        Name = "Alchemist";
        StartingTokenCount = 4;
    }

    public override int TallyPowerBonusVP(List<Region> regions)
    {
        return IsInDecline ? 0 : 2;
    }
}