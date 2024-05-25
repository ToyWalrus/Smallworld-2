using System.Collections.Generic;
using Smallworld.IO;

namespace Smallworld.Models.Powers;

public abstract class Power
{
    public string Name { get; protected set; }
    public int StartingTokenCount { get; protected set; }
    public bool IsInDecline { get; protected set; }

    public IConfirmation Confirmation { protected get; set; }
    public ISelection<Region> RegionSelection { protected get; set; }
    public ISelection<Player> PlayerSelection { protected get; set; }
    public IRollDice DiceRoller { protected get; set; }

    public Power()
    {
        IsInDecline = false;
    }

    public virtual void OnTurnStart() { }
    public virtual void OnTurnEnd() { }
    public virtual int TallyPowerBonusVP(List<Region> regions) => 0;
    public virtual int GetRegionConquerCostReduction(Region region) => 0;
    public virtual List<Token> GetRedeploymentTokens(List<Region> ownedRegions) => new();
    public virtual List<InvalidConquerReason> GetInvalidConquerReasons(List<Region> ownedRegions, Region region)
    {
        return region.GetInvalidConquerReasons(ownedRegions);
    }

    /// <summary>
    /// This method should be called before moving around
    /// any troops to and from regions.
    /// </summary>
    /// <param name="region">the conquered region.</param>
    public virtual void OnRegionConquered(Region region) { }

    public virtual void EnterDecline()
    {
        IsInDecline = true;
    }

    public override string ToString() => Name;
}
