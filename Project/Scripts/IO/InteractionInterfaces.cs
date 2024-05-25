using System.Collections.Generic;
using System.Threading.Tasks;
using Smallworld.Models;

namespace Smallworld.IO;

public interface IConfirmation
{
    Task<bool> ConfirmAsync(string message, string title = "Confirm", string confirmLabel = "Yes", string cancelLabel = "No");
}

public interface IRollDice
{
    Task<int> RollDiceAsync(IDice dice);
}

public interface ISelection<T>
{
    Task<T> SelectAsync(List<T> items);
}
