namespace Smallworld.Models.Races
{
    public class Halfling : Race
    {
        private int totalRegionsConquered;

        public Halfling() : base()
        {
            Name = "Halflings";
            StartingTokenCount = 6;
            MaxTokens = 11;
            totalRegionsConquered = 0;
        }

        public override void OnRegionConquered(Region region)
        {
            if (totalRegionsConquered < 2)
            {
                region.AddToken(Token.HoleInTheGround);
            }
            totalRegionsConquered++;
        }
    }
}