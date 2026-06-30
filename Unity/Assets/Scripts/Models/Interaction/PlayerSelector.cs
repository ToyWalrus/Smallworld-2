using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

using SMPlayer = Smallworld.Models.Player;

public class PlayerSelector : MonoBehaviour, ISelection<SMPlayer>
{
    [SerializeField] private GameUI GameUI;

    public Task<SMPlayer> SelectAsync(List<SMPlayer> items, Func<SMPlayer, string> getReason)
    {
        return SelectAsync(items, getReason, CancellationToken.None);
    }

    public async Task<SMPlayer> SelectAsync(List<SMPlayer> items, Func<SMPlayer, string> getReason, CancellationToken cancellationToken)
    {
        var tsc = new TaskCompletionSource<SMPlayer>(TaskCreationOptions.RunContinuationsAsynchronously);

        void listener(SMPlayer selection)
        {
            if (!items.Contains(selection))
            {
                GameUI.SetGameHintText($"Cannot select {selection.Name}:\n${getReason(selection)}");
                return;
            }

            Debug.Log($"Selected ${selection.Name}");
            tsc.TrySetResult(selection);
        }

        try
        {
            GameUI.AddPlayerButtonListener(listener);

            using (cancellationToken.Register(() => tsc.TrySetCanceled()))
            {
                return await tsc.Task;
            }
        }
        finally
        {
            GameUI.RemovePlayerButtonListener(listener);
        }
    }
}