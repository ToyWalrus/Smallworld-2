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

    public Region(RegionType type, RegionAttribute attribute, bool isBorder, RegionAttribute secondAttr = RegionAttribute.None)
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

    public Token GetSpecialTokens(out int count)
    {
        count = 1;
        if (HasToken(Token.Encampment))
        {
            count = tokens.Count((t) => t == Token.Encampment);
            return Token.Encampment;
        }
        if (HasToken(Token.Fortress))
        {
            count = tokens.Count((t) => t == Token.Fortress);
            return Token.Fortress;
        }
        if (HasToken(Token.TrollLair))
        {
            return Token.TrollLair;
        }
        if (HasToken(Token.Dragon))
        {
            return Token.Dragon;
        }
        if (HasToken(Token.HoleInTheGround))
        {
            return Token.HoleInTheGround;
        }
        if (HasToken(Token.Heroic))
        {
            return Token.Heroic;
        }
        count = 0;
        return Token.None;
    }

    public bool IsImmune()
    {
        return tokens.Exists((token) =>
            token == Token.Dragon ||
            token == Token.HoleInTheGround ||
            token == Token.Heroic);
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
    /// Sets a new RacePower in control of this region. Calls the OnWasConquered for the previous occupier
    /// and the OnNewRegionConquered for the new occupier.
    /// </summary>
    /// <param name="racePower">the new occupying race</param>
    /// <param name="conqueringTokenCount">the amount of tokens used to conquer this region</param>
    public void Conquer(RacePower racePower, int conqueringTokenCount)
    {
        if (OccupiedBy == racePower)
        {
            Logger.LogMessage($"Region was already conquered by {racePower.Name}");
            return;
        }

        if (OccupiedBy != null)
        {
            int troopReimbursement;

            if (OccupiedBy.IsInDecline)
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

        RemoveAllTokensOfType(Token.LostTribe);
        RemoveAllTokensOfType(Token.Race);
        tokens.AddRange(Enumerable.Repeat(Token.Race, conqueringTokenCount));

        OccupiedBy.OnNewRegionConquered(this, conqueringTokenCount);
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

    /// <summary>
    /// Returns the reasons why a region cannot be conquered. If the list is empty, the region can be conquered.
    /// </summary>
    public List<InvalidConquerReason> GetInvalidConquerReasons(List<Region> playerOwnedRegions)
    {
        var isFirstConquest = playerOwnedRegions.Count == 0;
        var reasons = new List<InvalidConquerReason>();

        if (IsImmune())
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
