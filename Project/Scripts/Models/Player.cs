using System;
using System.Collections.Generic;

namespace Smallworld.Models;

public class Player
{
    public string Name { get; set; }
    public int Score { get; set; }
    public List<RacePower> RacePowers => new(racePowers);

    private readonly List<RacePower> racePowers = new();

    public Player(string name)
    {
        Name = name;
        Score = 0;
    }

    public void AddScore(int score)
    {
        Score += score;
    }

    public void AddRacePower(RacePower racePower)
    {
        racePowers.Add(racePower);
    }

    public void RemoveRacePower(RacePower racePower)
    {
        racePowers.Remove(racePower);
    }
}
