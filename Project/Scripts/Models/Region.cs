using System.Collections.Generic;
using System.Linq;
using Smallworld.Models.Races;
using Smallworld.Utils;

namespace Smallworld.Models
{
    public class Region
    {
        public RegionType Type { get; private set; }
        public RegionAttribute Attribute { get; private set; }
        public RegionAttribute SecondAttribute { get; private set; }

        public bool IsBorder { get; set; }
        public bool IsOccupied => OccupiedBy != null || _tokens.Exists((t) => t == Token.LostTribe);
        public RacePower OccupiedBy { get; private set; }
        public List<Region> AdjacentTo { get; private set; }
        public int NumRaceTokens => _tokens.Count(t => t == Token.Race);

        private readonly List<Token> _tokens;

        public Region(RegionType type, RegionAttribute attribute, bool isBorder, RegionAttribute secondAttr = RegionAttribute.None)
        {
            Type = type;
            Attribute = attribute;
            SecondAttribute = secondAttr;
            IsBorder = isBorder;
            OccupiedBy = null;
            _tokens = new List<Token>();

            if (type == RegionType.Mountain)
            {
                _tokens.Add(Token.Mountain);
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
                count = _tokens.Where((t) => t == Token.Encampment).Count();
                return Token.Encampment;
            }
            if (HasToken(Token.Fortress))
            {
                count = _tokens.Where((t) => t == Token.Fortress).Count();
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
            if (HasToken(Token.Hero))
            {
                return Token.Hero;
            }
            count = 0;
            return Token.None;
        }

        public bool IsImmune()
        {
            return _tokens.Exists((token) =>
                token == Token.Dragon ||
                token == Token.HoleInTheGround ||
                token == Token.Hero);
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
            return baseCost + _tokens.Count;
        }

        /// <summary>
        /// Sets a new RacePower in control of this region.
        /// </summary>
        /// <param name="racePower">the new occupying race</param>
        /// <param name="numRaceTokens">the amount of tokens used to conquer this region</param>
        /// <returns>The number of reimbursible troops</returns>
        public int Conquer(RacePower racePower, int numRaceTokens)
        {
            if (OccupiedBy == racePower)
            {
                Logger.LogMessage($"Region was already conquered by {racePower.Name}");
                return 0;
            }

            int troopReimbursement = 0;
            if (IsOccupied && OccupiedBy != null)
            {
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
            }

            OccupiedBy = racePower;
            RemoveAllTokensOfType(Token.LostTribe);
            RemoveAllTokensOfType(Token.Race);

            for (int i = 0; i < numRaceTokens; ++i)
            {
                _tokens.Add(Token.Race);
            }

            return troopReimbursement;
        }

        public void Reinforce(int numRaceTokens)
        {
            for (int i = 0; i < numRaceTokens; ++i)
            {
                _tokens.Add(Token.Race);
            }
        }

        public void Abandon()
        {
            OccupiedBy = null;
            _tokens.Clear();

            if (Type == RegionType.Mountain)
            {
                _tokens.Add(Token.Mountain);
            }
        }

        /// <summary>
        /// Returns the number of troops available for conquesting from
        /// this region. (Number of race tokens minus 1)
        /// </summary>    
        public int GetExcessTroops() => System.Math.Max(0, NumRaceTokens - 1);
        public bool HasToken(Token token) => _tokens.Exists((t) => t == token);
        public void AddToken(Token token) => _tokens.Add(token);
        public void RemoveAllTokensOfType(Token tokenType) => _tokens.RemoveAll((t) => t == tokenType);

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

        public override bool Equals(object obj)
        {
            return obj is Region region &&
                Type == region.Type &&
                Attribute == region.Attribute &&
                SecondAttribute == region.SecondAttribute &&
                IsBorder == region.IsBorder;
        }

        public override int GetHashCode()
        {
            int hashCode = -612506139;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Attribute.GetHashCode();
            hashCode = hashCode * -1521134295 + SecondAttribute.GetHashCode();
            hashCode = hashCode * -1521134295 + IsBorder.GetHashCode();
            return hashCode;
        }
    }
}