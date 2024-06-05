using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Smallworld.Models.Powers;
using Smallworld.Models.Races;
using Smallworld.Utils;

namespace Smallworld.Models;

public interface IGame
{
    List<Player> Players { get; }
    List<Region> Regions { get; }
    List<RacePower> AvailableRacePowers { get; }
    int CurrentPlayerIndex { get; }
    int NumRounds { get; }

    void AddPlayer(Player player);
    void SetRegions(List<Region> regions);
    void SetAvailableRacePowers(List<RacePower> racePowers);
}

public partial class Game : IGame
{
    public List<Player> Players { get; private set; }
    public List<Region> Regions { get; private set; }
    public List<RacePower> AvailableRacePowers { get; private set; }
    public int CurrentPlayerIndex { get; set; }
    public int NumRounds { get; }

    private readonly HashSet<Type> usedPowers = new();
    private readonly HashSet<Type> usedRaces = new();

    private static IServiceProvider serviceProvider;

    public Game(IServiceProvider serviceProvider, int numRounds = 10)
    {
        Game.serviceProvider ??= serviceProvider;
        InitializePowersAndRaces();

        Players = new List<Player>();
        Regions = new List<Region>();
        AvailableRacePowers = new List<RacePower>();
        NumRounds = numRounds;
    }

    public void AddPlayer(Player player)
    {
        Players.Add(player);
    }

    public void SetRegions(List<Region> regions)
    {
        Regions = regions;
    }

    public void SetAvailableRacePowers(List<RacePower> racePowers)
    {
        AvailableRacePowers = racePowers;
    }

    public RacePower GenerateNewRacePower(bool unused = true)
    {
        var power = GetRandomPower(unused);
        var race = GetRandomRace(unused);
        return new RacePower(race, power);
    }

    private Power GetRandomPower(bool unused)
    {
        var powerFactory = serviceProvider.GetRequiredService<IModelFactory<Power>>();
        var powers = unused ? allPowers.Where(p => !usedPowers.Contains(p)) : allPowers;

        if (!powers.Any())
        {
            Logger.LogMessage("No more unused powers available, resetting used list");
            usedPowers.Clear();
        }

        Type randomPower = powers.ElementAt(new Random().Next(0, powers.Count()));

        usedPowers.Add(randomPower);

        return powerFactory.Create(randomPower);
    }

    private Race GetRandomRace(bool unused)
    {
        var raceFactory = serviceProvider.GetRequiredService<IModelFactory<Race>>();
        var races = unused ? allRaces.Where(r => !usedRaces.Contains(r)) : allRaces;

        if (!races.Any())
        {
            Logger.LogMessage("No more unused races available, resetting used list");
            usedRaces.Clear();
        }

        Type randomRace = races.ElementAt(new Random().Next(0, races.Count()));

        usedRaces.Add(randomRace);

        return raceFactory.Create(randomRace);
    }

    private static IEnumerable<Type> allPowers;
    private static IEnumerable<Type> allRaces;
    private static void InitializePowersAndRaces()
    {
        if (allPowers != null && allRaces != null)
        {
            return;
        }

        allPowers = GetEnumerableOfType<Power>();
        allRaces = GetEnumerableOfType<Race>();
    }
    private static IEnumerable<Type> GetEnumerableOfType<T>() where T : class
    {
        var objects = new List<Type>();
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
                {
                    objects.Add(type);
                }
            }
        }
        return objects;
    }
}
