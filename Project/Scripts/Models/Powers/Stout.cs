using System.Threading.Tasks;

namespace Smallworld.Models.Powers;

public class Stout : Power
{
    public Stout()
    {
        Name = "Stout";
        StartingTokenCount = 4;
    }

    public override async Task OnTurnEnd()
    {
        var confirmed = await Confirmation.ConfirmAsync("Would you like to enter decline?");
        if (confirmed)
        {
            racePower.EnterDecline();
        }
    }

    public override bool CanEnterDecline() => !IsInDecline;
}
