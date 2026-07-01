using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ResourceScripts;
using SMRegion = Smallworld.Models.Region;

[Tool]
public partial class Region : Node2D
{
	[Export]
	private RegionResource _regionResource;

	[Export]
	public Region[] AdjacentRegions
	{
		get => _adjacentRegions;
		set
		{
			_adjacentRegions = value ?? [];
			SyncAdjacency();
		}
	}
	private Region[] _adjacentRegions = [];
	private Region[] _oldAdjacentRegions = [];

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public SMRegion GetModel()
	{
		return _regionResource.GetModel();
	}

	private static bool _syncing;
	private void SyncAdjacency()
	{
		if (_regionResource != null)
		{
			_regionResource.AdjacentRegions = [.. _adjacentRegions.Select(r => r._regionResource)];
		}


		if (_syncing) return;

		try
		{
			_syncing = true;

			_adjacentRegions ??= [];
			_oldAdjacentRegions ??= [];

			var current = new HashSet<Region>(_adjacentRegions.Where(r => r != null && r != this));
			var previous = new HashSet<Region>(_oldAdjacentRegions.Where(r => r != null && r != this));

			// Update neighbors
			foreach (var added in current.Except(previous))
			{
				added.AdjacentRegions ??= [];

				if (!added._adjacentRegions.Contains(this))
				{
					added.AdjacentRegions = [.. added._adjacentRegions, this];
				}
			}

			// Remove neighbors
			foreach (var removed in previous.Except(current))
			{
				if (removed._adjacentRegions != null && removed._adjacentRegions.Contains(this))
				{
					removed._adjacentRegions = [.. removed._adjacentRegions.Except([this])];
				}
			}

			// Update snapshot
			_oldAdjacentRegions = [.. _adjacentRegions];
		}
		finally
		{
			_syncing = false;
		}
	}
}
