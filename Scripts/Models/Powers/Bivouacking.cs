namespace Smallworld.Models.Powers
{
    public class Bivouacking : Power
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