using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

using SMPlayer = Smallworld.Models.Player;

public class PlayerSelector : MonoBehaviour, ISelection<SMPlayer>
{
    public Task<SMPlayer> SelectAsync(List<SMPlayer> items)
    {
        throw new System.NotImplementedException();
    }

    public Task<SMPlayer> SelectAsync(List<SMPlayer> items, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}