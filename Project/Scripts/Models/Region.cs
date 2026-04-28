using System.Collections.Generic;
using System.Linq;
using Smallworld.Models.Races;
using Smallworld.Utils;

namespace Smallworld.Models;

public enum InvalidConquerReason
{
    NotBorder,
    SeaOrLake,
    NotAdjacent,
    RegionImmune,
    IsOwnedBySelf,
    ProtectedByDiplomat,
}

public class Region
{
    public string Name { get; set; }
    public RegionType Type { get; private set; }
    public RegionAttribute Attribute { get; private set; }
    public RegionAttribute SecondAttribute { get; private set; }

    public bool IsBorder { get; set; }
    public bool IsOccupied => OccupiedBy != null || HasToken(Token.LostTribe);
    public RacePower OccupiedBy { get; private set; }
    public List<Region> AdjacentTo { get; private set; }
    public int NumRaceTokens => tokens.Count(t => t == Token.Race);

    private readonly List<Token> tokens;
    private bool isImmune;

    public Region(RegionType type, RegionAttribute attribute = RegionAttribute.None, bool isBorder = false, RegionAttribute secondAttr = RegionAttribute.None)
    {
        Type = type;
        Attribute = attribute;
        SecondAttribute = secondAttr;
        IsBorder = isBorder;
        OccupiedBy = null;
        AdjacentTo = new();

        tokens = new();

        if (type == RegionType.Mountain)
        {
            tokens.Add(Token.Mountain);
        }
    }

    public bool HasAttribute(RegionAttribute attr)
    {
        return Attribute == attr || SecondAttribute == attr;
    }

    public void SetAdjacentRegions(List<Region> regions)
    {
        AdjacentTo = new List<Region>(regions);
        if (AdjacentTo.Contains(null))
        {
            Logger.LogWarning("An adjacent region is null! This region: " + Type + ", " + Attribute + ", " + AdjacentTo.Count + " adjacent regions");
        }
    }

    public (Token, int) GetSpecialTokens()
    {
        if (HasToken(Token.Encampment))
        {
            return (Token.Encampment, tokens.Count((t) => t == Token.Encampment));
        }
        if (HasToken(Token.Fortress))
        {
            return (Token.Fortress, tokens.Count((t) => t == Token.Fortress));
        }
        if (HasToken(Token.TrollLair))
        {
            return (Token.TrollLair, 1);
        }
        if (HasToken(Token.Dragon))
        {
            return (Token.Dragon, 1);
        }
        if (HasToken(Token.HoleInTheGround))
        {
            return (Token.HoleInTheGround, 1);
        }
        if (HasToken(Token.Heroic))
        {
            return (Token.Heroic, 1);
        }
        return (Token.None, 0);
    }

    /// <summary>
    /// Returns the base number of tokens needed to conquer this
    /// region. (This does not factor in Race or Power conquering
    /// cost reductions)
    /// </summary>
    /// <returns></returns>
    public int GetBaseConquerCost()
    {
        int baseCost = 2;
        return baseCost + tokens.Count;
    }

    /// <summary>
    /// Sets a new RacePower in control of this region. Calls the OnWasConquered for the previous occupier.
    /// </summary>
    /// <param name="racePower">the new occupying race</param>
    /// <param name="conqueringTokenCount">the amount of tokens used to conquer this region</param>
    public void WasConquered(RacePower racePower, int conqueringTokenCount)
    {
        if (OccupiedBy == racePower)
        {
            Logger.LogMessage($"Region was already conquered by {racePower.Name}");
            return;
        }

        if (OccupiedBy != null)
        {
            int troopReimbursement;

            if (OccupiedBy.IsInDecline && OccupiedBy.Race is not Ghoul)
            {
                troopReimbursement = 0;
            }
            else if (OccupiedBy.Race is Elf)
            {
                troopReimbursement = NumRaceTokens;
            }
            else
            {
                troopReimbursement = NumRaceTokens - 1;
            }

            OccupiedBy.OnWasConquered(this, troopReimbursement);
        }

        OccupiedBy = racePower;
        isImmune = false;

        RemoveAllTokensOfType(Token.LostTribe);
        RemoveAllTokensOfType(Token.Race);
        tokens.AddRange(Enumerable.Repeat(Token.Race, conqueringTokenCount));
    }

    public void Reinforce(int numRaceTokens)
    {
        for (int i = 0; i < numRaceTokens; ++i)
        {
            tokens.Add(Token.Race);
        }
    }

    public void Abandon()
    {
        OccupiedBy = null;
        isImmune = false;
        tokens.Clear();

        if (Type == RegionType.Mountain)
        {
            tokens.Add(Token.Mountain);
        }
    }

    public void ClearExcessRaceTokens()
    {
        var excess = GetExcessRaceTokens();
        while (excess > 0)
        {
            tokens.Remove(Token.Race);
            excess--;
        }
    }

    /// <summary>
    /// Returns the number of troops available for conquesting from
    /// this region. (Number of race tokens minus 1)
    /// </summary>    
    public int GetExcessRaceTokens() => System.Math.Max(0, NumRaceTokens - 1);
    public bool HasToken(Token token) => tokens.Exists((t) => t == token);
    public void AddToken(Token token) => tokens.Add(token);
    public void RemoveAllTokensOfType(Token tokenType) => tokens.RemoveAll((t) => t == tokenType);
    public void SetImmune(bool immune) => isImmune = immune;

    /// <summary>
    /// Determines whether this region is able to be conquered by the provided RacePower.
    /// </summary>
    /// <param name="rp">The RacePower checking whether this region is conquerable.</param>
    /// <returns>A tuple, boolean first and if it is false, the second item is the reason as a string</returns>
    public (bool, string) IsValidConquerTarget(RacePower rp)
    {
        var restrictions = GetConquerRestrictions(rp.GetOwnedRegions());

        rp.ModifyConquerRestrictions(restrictions, this);
        OccupiedBy?.ModifyDefenseRestrictions(restrictions, rp, this);

        if (!restrictions.Any())
        {
            if (rp.AvailableTokenCount < rp.EstimateRegionConquerCost(this))
            {
                return (false, "Not enough tokens");
            }

            return (true, "");
        }

        string reason = "| ";
        foreach (var currentReason in restrictions)
        {
            switch (currentReason)
            {
                case InvalidConquerReason.IsOwnedBySelf:
                    reason += "Region is already owned by player | ";
                    break;
                case InvalidConquerReason.NotAdjacent:
                    reason += "Region is not adjacent to any owned regions | ";
                    break;
                case InvalidConquerReason.SeaOrLake:
                    reason += "Region is a sea or lake | ";
                    break;
                case InvalidConquerReason.NotBorder:
                    reason += "First conquest must happen on a border region | ";
                    break;
                case InvalidConquerReason.RegionImmune:
                    reason += "Region is immune to conquest | ";
                    break;
                case InvalidConquerReason.ProtectedByDiplomat:
                    reason += "Region is protected by a Diplomat | ";
                    break;
            }
        }

        return (false, reason.Trim());
    }

    /// <summary>
    /// Returns the reasons why a region cannot be conquered. If the list is empty, the region can be conquered.
    /// </summary>
    public List<InvalidConquerReason> GetConquerRestrictions(List<Region> playerOwnedRegions)
    {
        var isFirstConquest = playerOwnedRegions.Count == 0;
        var reasons = new List<InvalidConquerReason>();

        if (playerOwnedRegions.Contains(this))
        {
            reasons.Add(InvalidConquerReason.IsOwnedBySelf);
            return reasons;
        }

        if (isImmune)
        {
            reasons.Add(InvalidConquerReason.RegionImmune);
        }

        if (playerOwnedRegions.Count > 0 && !playerOwnedRegions.Any(r => IsAdjacentTo(r)))
        {
            reasons.Add(InvalidConquerReason.NotAdjacent);
        }

        if (isFirstConquest && !IsBorder)
        {
            reasons.Add(InvalidConquerReason.NotBorder);
        }

        if (Type == RegionType.Sea || Type == RegionType.Lake)
        {
            reasons.Add(InvalidConquerReason.SeaOrLake);
        }

        return reasons;
    }

    public bool IsAdjacentTo(Region region)
    {
        return AdjacentTo.Contains(region);
    }

    public bool IsAdjacentTo(RegionType type)
    {
        foreach (Region region in AdjacentTo)
        {
            if (region.Type == type)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAdjacentTo(RegionAttribute attribute)
    {
        foreach (Region region in AdjacentTo)
        {
            if (region.HasAttribute(attribute))
            {
                return true;
            }
        }
        return false;
    }

    override public string ToString()
    {
        var str = GetRegionAttributeString(Attribute) + GetRegionAttributeString(SecondAttribute);

        switch (Type)
        {
            case RegionType.Forest:
                str += " Forest";
                break;
            case RegionType.Hill:
                str += " Hill";
                break;
            case RegionType.Mountain:
                str += " Mountain";
                break;
            case RegionType.Swamp:
                str += " Swamp";
                break;
            case RegionType.Sea:
                str += " Sea";
                break;
            case RegionType.Lake:
                str += " Lake";
                break;
        }

        if (Name == "")
        {
            return str.Trim();
        }
        if (str == "")
        {
            return Name;
        }
        return $"{Name} ({str.Trim()})";
    }


    private static string GetRegionAttributeString(RegionAttribute attr)
    {
        return attr switch
        {
            RegionAttribute.Underworld => " Underworld",
            RegionAttribute.Magic => " Magic",
            RegionAttribute.Mine => " Mine",
            _ => "",
        };
    }
}
