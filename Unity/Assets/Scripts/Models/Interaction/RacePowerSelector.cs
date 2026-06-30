using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

using SMRacePower = Smallworld.Models.RacePower;

public class RacePowerSelector : MonoBehaviour, ISelection<SMRacePower>
{
    [SerializeField] private GameUI GameUI;

    void Awake()
    {
        GameUI.SetRacePowerButtonsInteractable(false);
    }

    public Task<SMRacePower> SelectAsync(List<SMRacePower> items, Func<SMRacePower, string> getReason)
    {
        return SelectAsync(items, getReason, CancellationToken.None);
    }

    public async Task<SMRacePower> SelectAsync(List<SMRacePower> items, Func<SMRacePower, string> getReason, CancellationToken cancellationToken)
    {
        var tsc = new TaskCompletionSource<SMRacePower>(TaskCreationOptions.RunContinuationsAsynchronously);


        void listener(SMRacePower selection)
        {
            if (!items.Contains(selection))
            {
                GameUI.SetGameHintText($"Cannot select {selection.Name}:\n{getReason(selection)}");
                return;
            }

            tsc.TrySetResult(selection);
        }

        try
        {
            GameUI.AddRacePowerButtonListener(listener);
            GameUI.SetRacePowerButtonsInteractable(true);

            using (cancellationToken.Register(() => tsc.TrySetCanceled()))
            {
                return await tsc.Task;
            }
        }
        finally
        {
            GameUI.SetRacePowerButtonsInteractable(false);
            GameUI.RemoveRacePowerButtonListener(listener);
        }
    }
}