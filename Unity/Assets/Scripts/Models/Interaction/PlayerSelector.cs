using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

using SMPlayer = Smallworld.Models.Player;

public class PlayerSelector : MonoBehaviour, ISelection<SMPlayer>
{
    public GameUI gameUI;

    public Task<SMPlayer> SelectAsync(List<SMPlayer> items)
    {
        return SelectAsync(items, CancellationToken.None);
    }

    public async Task<SMPlayer> SelectAsync(List<SMPlayer> items, CancellationToken cancellationToken)
    {
        var tsc = new TaskCompletionSource<SMPlayer>(TaskCreationOptions.RunContinuationsAsynchronously);

        void listener(SMPlayer selection)
        {
            Debug.Log($"Selected ${selection.Name}");
            tsc.TrySetResult(selection);
        }

        try
        {
            gameUI.AddPlayerButtonListener(listener);

            using (cancellationToken.Register(() => tsc.TrySetCanceled()))
            {
                return await tsc.Task;
            }
        }
        finally
        {
            gameUI.RemovePlayerButtonListener(listener);
        }
    }
}