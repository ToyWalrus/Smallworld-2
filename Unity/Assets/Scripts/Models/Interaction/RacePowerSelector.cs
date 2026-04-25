using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;

using SMRacePower = Smallworld.Models.RacePower;

public class RacePowerSelector : MonoBehaviour, ISelection<SMRacePower>
{
    public GameUI gameUI;

    void Start()
    {
        gameUI.SetRacePowerButtonsInteractable(false);
    }

    public Task<SMRacePower> SelectAsync(List<SMRacePower> items)
    {
        return SelectAsync(items, CancellationToken.None);
    }

    public async Task<SMRacePower> SelectAsync(List<SMRacePower> items, CancellationToken cancellationToken)
    {
        var tsc = new TaskCompletionSource<SMRacePower>(TaskCreationOptions.RunContinuationsAsynchronously);


        void listener(SMRacePower selection)
        {
            tsc.TrySetResult(selection);
        }

        try
        {
            gameUI.SetRacePowerButtons(items);
            gameUI.AddRacePowerButtonListener(listener);
            gameUI.SetRacePowerButtonsInteractable(true);

            using (cancellationToken.Register(() => tsc.TrySetCanceled()))
            {
                return await tsc.Task;
            }
        }
        finally
        {
            gameUI.SetRacePowerButtonsInteractable(false);
            gameUI.RemoveRacePowerButtonListener(listener);
        }
    }
}