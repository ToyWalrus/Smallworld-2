using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models;

public class Player
{
    public string Name { get; set; }
    public int Score { get; private set; }
    public List<RacePower> RacePowers => new(racePowers);
    public bool HasActiveRace => ActiveRacePower != null;
    public RacePower ActiveRacePower => RacePowers.FirstOrDefault(rp => !rp.IsInDecline);

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
        racePower.SetOwner(this);
    }

    public void RemoveRacePower(RacePower racePower)
    {
        racePowers.Remove(racePower);
        racePower.SetOwner(null);
    }

    public void ClearDeclineRacePowers()
    {
        foreach (var rp in racePowers.Where(rp => rp.IsInDecline))
        {
            rp.AbandonAllRegions();
            RemoveRacePower(rp);
        }
    }

    public int TallyVP()
    {
        return racePowers.Sum(rp => rp.TallyVP());
    }

    public override string ToString()
    {
        return Name;
    }
}
