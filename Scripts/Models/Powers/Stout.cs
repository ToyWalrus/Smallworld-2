using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smallworld.Models.Powers
{
  class Stout : Power
  {
    public Stout()
    {
      Name = "Stout";
      StartingTokenCount = 4;
    }

    public override Task OnTurnEnd(List<Region> ownedRegions)
    {
      // prompt user whether to enter decline
      // if yes,
      // _racePower.EnterDecline();
      return Task.CompletedTask; // only placeholder task for now
    }
  }
}