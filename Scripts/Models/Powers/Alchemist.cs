using System.Collections.Generic;

namespace Smallworld.Models.Powers
{
  class Alchemist : Power
  {
    public Alchemist()
    {
      Name = "Alchemist";
      StartingTokenCount = 4;
    }

    public override int TallyPowerBonusVP(List<Region> regions)
    {
      return 2;
    }
  }
}