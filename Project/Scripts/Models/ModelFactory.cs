using System;
using Microsoft.Extensions.DependencyInjection;
using Smallworld.IO;
using Smallworld.Models.Powers;
using Smallworld.Models.Races;

namespace Smallworld.Models;

public interface IModelFactory<M>
{
    T Create<T>() where T : M;
    M Create(Type type);
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
        return (T)Create(typeof(T));
    }

    public Power Create(Type type)
    {
        var newPower = (Power)Activator.CreateInstance(type);
        newPower.Confirmation = _serviceProvider.GetRequiredService<IConfirmation>();
        newPower.PlayerSelection = _serviceProvider.GetRequiredService<ISelection<Player>>();
        newPower.DiceRoller = _serviceProvider.GetRequiredService<IRollDice>();
        return newPower;
    }
}

public class RaceFactory : IModelFactory<Race>
{
    private readonly IServiceProvider _serviceProvider;

    public RaceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public T Create<T>() where T : Race
    {
        return (T)Create(typeof(T));
    }

    public Race Create(Type type)
    {
        var newRace = (Race)Activator.CreateInstance(type);
        // newRace.RegionSelection = _serviceProvider.GetRequiredService<ISelection<Region>>();
        return newRace;
    }
}