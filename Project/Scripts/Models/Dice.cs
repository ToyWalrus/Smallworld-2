using System;
using System.Collections.Generic;
using System.Linq;

namespace Smallworld.Models;

public interface IDice
{
    int Roll();
}

public class EqualProbabilityDice : IDice
{
    public int Sides { get; set; }
    public int Value { get; set; }

    public EqualProbabilityDice(int sides)
    {
        Sides = sides;
        Value = 0;
    }

    public EqualProbabilityDice(int sides, int value)
    {
        Sides = sides;
        Value = value;
    }

    public int Roll()
    {
        Value = new Random().Next(1, Sides + 1);
        return Value;
    }
}

public class CustomProbabilityDice : IDice
{
    public List<int> Distribution { get; set; }
    public int Value { get; set; }

    public CustomProbabilityDice(List<int> distribution)
    {
        Distribution = distribution;
        Value = 0;
    }

    public int Roll()
    {
        Value = Distribution.ElementAt(new Random().Next(0, Distribution.Count));
        return Value;
    }
}