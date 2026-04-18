using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

using SMRacePower = Smallworld.Models.RacePower;

public class RacePowerSelector : MonoBehaviour, ISelection<SMRacePower>
{
    public Task<SMRacePower> SelectAsync(List<SMRacePower> items)
    {
        throw new System.NotImplementedException();
    }

    public Task<SMRacePower> SelectAsync(List<SMRacePower> items, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}