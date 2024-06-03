using Smallworld.IO;
using Smallworld.Models;

namespace Tests;

internal class ConfirmationMock : IConfirmation
{
    private bool shouldConfirm = true;

    public void SetShouldConfirm(bool shouldConfirm)
    {
        this.shouldConfirm = shouldConfirm;
    }

    public Task<bool> ConfirmAsync(string message, string title = "Confirm", string confirmLabel = "Yes", string cancelLabel = "No")
    {
        return Task.FromResult(shouldConfirm);
    }
}

internal class RollDiceMock : IRollDice
{
    private int value;

    public RollDiceMock(int value)
    {
        this.value = value;
    }

    public void SetValue(int value)
    {
        this.value = value;
    }

    public Task<int> RollDiceAsync()
    {
        return Task.FromResult(value);
    }
}

internal class SelectionMock<T> : ISelection<T>
{
    private T item;

    public SelectionMock(T item)
    {
        this.item = item;
    }

    public Task<T> SelectAsync(List<T> items)
    {
        return Task.FromResult(item);
    }
}