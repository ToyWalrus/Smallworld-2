using System.Threading.Tasks;

namespace Smallworld.Models.Races
{
    public class Troll : Race
    {
        public Troll() : base()
        {
            Name = "Trolls";
            StartingTokenCount = 5;
            MaxTokens = 10;
        }

        public override Task OnRegionConquered(Region region)
        {
            region.AddToken(Token.TrollLair);
            return Task.CompletedTask;
        }
    }
}