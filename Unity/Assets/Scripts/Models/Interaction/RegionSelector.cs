using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Smallworld.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityModels;

using SMRegion = Smallworld.Models.Region;

public class RegionSelector : MonoBehaviour, ISelection<SMRegion>
{
    private TaskCompletionSource<SMRegion> tsc;
    private List<SMRegion> validItems;

    public Task<SMRegion> SelectAsync(List<SMRegion> items)
    {
        return SelectAsync(items, CancellationToken.None);
    }

    public async Task<SMRegion> SelectAsync(List<SMRegion> items, CancellationToken cancellationToken)
    {
        if (tsc != null)
        {
            Debug.LogError("Already in selection!");
            return null;
        }

        tsc = new(TaskCreationOptions.RunContinuationsAsynchronously);
        validItems = items;

        try
        {
            using (cancellationToken.Register(() => tsc.TrySetCanceled()))
            {
                return await tsc.Task;
            }
        }
        finally
        {
            validItems = null;
            tsc = null;
        }
    }

    void Update()
    {
        if (tsc == null || validItems == null)
        {
            return;
        }

        var mouse = Mouse.current;
        if (!mouse.leftButton.wasPressedThisFrame)
        {
            return;
        }

        var ray = Camera.main.ScreenPointToRay(mouse.position.value);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Region")))
        {
            var region = hit.collider.GetComponent<Region>();
            if (region)
            {
                if (validItems.Contains(region.GetModel()))
                {
                    tsc.TrySetResult(region.GetModel());
                }
                else
                {
                    Debug.Log("That is not a valid region to select");
                }
            }
        }
    }
}