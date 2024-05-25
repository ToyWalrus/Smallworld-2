using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models.Races;

public abstract class Race
{
    public string Name { get; protected set; }
    public int MaxTokens { get; protected set; }
    public int StartingTokenCount { get; protected set; }
    public bool IsInDecline { get; protected set; }

    public Race()
    {
        IsInDecline = false;
    }

    public virtual void OnTurnStart() { }
    public virtual void OnTurnEnd() { }
    public virtual void OnRegionConquered(Region region) { }
    public virtual int GetRegionConquerCostReduction(Region region) => 0;
    public virtual int TallyRaceBonusVP(List<Region> ownedRegions) => 0;
    public virtual List<InvalidConquerReason> GetInvalidConquerReasons(List<Region> ownedRegions, Region region)
    {
        return region.GetInvalidConquerReasons(ownedRegions);
    }

    public virtual List<Token> GetRedeploymentTokens(List<Region> ownedRegions)
    {
        var totalRaceTokens = ownedRegions.Sum(r => r.NumRaceTokens);

        // Need to leave at least one token in each region
        var length = System.Math.Max(0, totalRaceTokens - ownedRegions.Count);

        return Enumerable.Repeat(Token.Race, length).ToList();
    }

    public virtual void EnterDecline()
    {
        IsInDecline = true;
    }
}
