using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Smallworld.Models;
using SMRegion = Smallworld.Models.Region;

namespace ResourceScripts;

[Tool, GlobalClass]
public partial class Region : Resource
{
	private static bool _syncing;

	private SMRegion _model = null;

	[Export]
	public RegionType Type
	{
		get => _type; set
		{
			_type = value;
			UpdateResource();
		}
	}
	private RegionType _type = RegionType.Sea;

	[Export]
	public RegionAttribute PrimaryAttribute
	{
		get => _primaryAttribute; set
		{
			_primaryAttribute = value;
			UpdateResource();
		}
	}
	private RegionAttribute _primaryAttribute = RegionAttribute.None;

	[Export]
	public RegionAttribute SecondaryAttribute
	{
		get => _secondaryAttribute; set
		{
			_secondaryAttribute = value;
			UpdateResource();
		}
	}
	private RegionAttribute _secondaryAttribute = RegionAttribute.None;

	[Export]
	public bool IsBorder
	{
		get => _isBorder; set
		{
			_isBorder = value;
			UpdateResource();
		}
	}
	private bool _isBorder = false;

	[Export]
	public bool IsLostTribeRegion
	{
		get => _isLostTribeRegion; set
		{
			_isLostTribeRegion = value;
			UpdateResource();
		}
	}
	private bool _isLostTribeRegion = false;

	[Export]
	public Region[] AdjacentRegions
	{
		get => _adjacentRegions;
		set
		{
			_adjacentRegions = value;
			UpdateResource();
		}
	}
	private Region[] _adjacentRegions = [];

	private Region[] _oldAdjacentRegions = [];

	private void UpdateResource()
	{
		SyncAdjacency();
		RebuildModel();
	}

	private void RebuildModel()
	{
		_model = new(ResourceName, Type, PrimaryAttribute, IsBorder, SecondaryAttribute, IsLostTribeRegion);

		if (_adjacentRegions != null)
		{
			_model.SetAdjacentRegions(
				[.. AdjacentRegions
					.Where(r => r != null)
					.Select(r => r.GetModel())]
			);
		}
	}

	private void SyncAdjacency()
	{
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
				added._adjacentRegions ??= [];

				if (!added._adjacentRegions.Contains(this))
				{
					added._adjacentRegions = [.. added._adjacentRegions, this];
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

	public SMRegion GetModel()
	{
		if (_model == null)
		{
			RebuildModel();
		}
		return _model;
	}
}
