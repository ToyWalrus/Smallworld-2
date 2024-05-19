using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smallworld.Models.Powers
{
  class Bivouacking : Power
  {
    public Bivouacking()
    {
      Name = "Bivouacking";
      StartingTokenCount = 5;
    }

    public override Task OnTurnEnd(List<Region> ownedRegions)
    {
      // prompt user where to put encampments
      return Task.CompletedTask; // placeholder for prompt
    }
  }
}