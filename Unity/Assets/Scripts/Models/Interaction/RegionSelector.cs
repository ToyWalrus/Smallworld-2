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
    [SerializeField] private bool AlwaysShowHoverState = false;

    private TaskCompletionSource<SMRegion> tsc;
    private List<SMRegion> validItems;
    private Region previouslyHovered;

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
        HandleRegionHovering();
    }

    private void HandleRegionHovering()
    {
        var mouse = Mouse.current;
        var ray = Camera.main.ScreenPointToRay(mouse.position.value);

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMask.GetMask("Region")))
        {
            ClearPreviouslyHovered();
            return;
        }

        if (!hit.collider.TryGetComponent<Region>(out var region))
        {
            Debug.LogError("Detected a non-region component on the Region layer!");
            return;
        }

        if (AlwaysShowHoverState)
        {
            UpdateHoverState(region);
        }

        if (validItems == null || !validItems.Contains(region.GetModel()))
        {
            return;
        }

        // Update hover state here if not already updated
        if (!AlwaysShowHoverState)
        {
            UpdateHoverState(region);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && tsc != null)
        {
            tsc.TrySetResult(region.GetModel());
        }
    }

    private void UpdateHoverState(Region region)
    {
        if (previouslyHovered != null && previouslyHovered != region)
        {
            previouslyHovered.SetIsHovered(false);
        }

        previouslyHovered = region;
        region.SetIsHovered(true);
    }

    private void ClearPreviouslyHovered()
    {
        if (previouslyHovered != null)
        {
            previouslyHovered.SetIsHovered(false);
            previouslyHovered = null;
        }
    }
}