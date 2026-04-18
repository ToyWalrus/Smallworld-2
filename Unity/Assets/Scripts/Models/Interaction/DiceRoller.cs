using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

public class DiceRoller : MonoBehaviour, IRollDice
{
    public List<int> distribution = new(new int[] { 0, 0, 0, 1, 2, 3 });

    public int GetMaxRollValue()
    {
        return distribution.Max();
    }

    public Task<int> RollDiceAsync()
    {
        throw new System.NotImplementedException();
    }
}