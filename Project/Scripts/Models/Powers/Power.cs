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
    public virtual Task<int> GetRegionConquerCostReduction(Region region) => Task.FromResult(0);
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
    public virtual void OnRegionConquered(Region region)
    {
        canEnterDecline = false;
    }

    public virtual bool CanEnterDecline() => canEnterDecline;

    public virtual void EnterDecline()
    {
        canEnterDecline = false;
        IsInDecline = true;
        OnEnterDecline(racePower.GetOwnedRegions());
    }

    protected virtual void OnEnterDecline(List<Region> ownedRegions) { }

    public override string ToString() => Name;
}
