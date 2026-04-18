using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

using SMRegion = Smallworld.Models.Region;

public class RegionSelector : MonoBehaviour, ISelection<SMRegion>
{
    public Task<SMRegion> SelectAsync(List<SMRegion> items)
    {
        throw new System.NotImplementedException();
    }

    public Task<SMRegion> SelectAsync(List<SMRegion> items, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}