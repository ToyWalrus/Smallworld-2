using System.Collections.Generic;
using System.Threading.Tasks;
using Smallworld.IO;

namespace Smallworld.Models.Powers;

public abstract class Power
{
    public string Name { get; protected set; }
    public int StartingTokenCount { get; protected set; }
    public bool IsInDecline { get; protected set; }
    protected bool canEnterDecline = true;

    public IConfirmation Confirmation { protected get; set; }
    public ISelection<Player> PlayerSelection { protected get; set; }
    public IRollDice DiceRoller { protected get; set; }
    public IGame GameRef { protected get; set; }

    protected RacePower racePower;

    public Power()
    {
        IsInDecline = false;
    }

    // Not great -- tightly couples this to RacePower. Should find another way (only used by Stout power)
    public void SetRacePower(RacePower racePower)
    {
        this.racePower = racePower;
    }

    public virtual void OnTurnStart()
    {
        canEnterDecline = true;
    }
    public virtual Task OnTurnEnd() => Task.CompletedTask;
    public virtual int TallyPowerBonusVP(List<Region> regions) => 0;
    public virtual int GetEstimatedConquerCostReduction(Region region) => 0;
    public virtual Task<int> GetRegionConquerCostReduction(Region region) => Task.FromResult(GetEstimatedConquerCostReduction(region));
    public virtual List<Token> GetRedeploymentTokens(List<Region> ownedRegions) => new();

    /// <summary>
    /// Called when this power is the attacker. Override to remove reasons from <paramref name="reasons"/>
    /// to allow conquests that would otherwise be blocked.
    /// </summary>
    public virtual void ModifyConquerRestrictions(List<InvalidConquerReason> reasons, List<Region> ownedRegions, Region region) { }

    /// <summary>
    /// Called when this power occupies the region being attacked. Override to add reasons to <paramref name="reasons"/>
    /// to block conquests that would otherwise be allowed.
    /// </summary>
    public virtual void ModifyDefenseRestrictions(List<InvalidConquerReason> reasons, RacePower attacker, Region region) { }

    /// <summary>
    /// This method should be called before moving around
    /// any troops to and from regions.
    /// </summary>
    /// <param name="region">the conquered region.</param>
    public virtual void OnRegionConquered(Region region)
    {
        canEnterDecline = false;
    }

    public virtual bool CanEnterDecline() => canEnterDecline;

    public virtual void EnterDecline()
    {
        canEnterDecline = false;
        IsInDecline = true;
        OnEnterDecline(racePower?.GetOwnedRegions() ?? new List<Region>());
    }

    protected virtual void OnEnterDecline(List<Region> ownedRegions) { }

    public override string ToString() => Name;
}
