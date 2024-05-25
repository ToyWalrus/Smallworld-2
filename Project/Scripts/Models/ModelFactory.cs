using System;
using Microsoft.Extensions.DependencyInjection;
using Smallworld.IO;
using Smallworld.Models.Powers;

namespace Smallworld.Models;

public interface IModelFactory<M>
{
    T Create<T>() where T : M;
}

public class PowerFactory : IModelFactory<Power>
{
    private readonly IServiceProvider _serviceProvider;

    public PowerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Create<T>() where T : Power
    {
        var newPower = Activator.CreateInstance<T>();
        newPower.Confirmation = _serviceProvider.GetRequiredService<IConfirmation>();
        newPower.RegionSelection = _serviceProvider.GetRequiredService<ISelection<Region>>();
        newPower.PlayerSelection = _serviceProvider.GetRequiredService<ISelection<Player>>();
        newPower.DiceRoller = _serviceProvider.GetRequiredService<IRollDice>();
        return newPower;
    }
}