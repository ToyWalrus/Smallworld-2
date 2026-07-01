using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Smallworld.Models;
using SMRegion = Smallworld.Models.Region;

namespace ResourceScripts;

[Tool, GlobalClass]
public partial class RegionResource : Resource
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
	public RegionResource[] AdjacentRegions
	{
		get => _adjacentRegions;
		set
		{
			_adjacentRegions = value;
			UpdateResource();
		}
	}
	private RegionResource[] _adjacentRegions = [];

	private RegionResource[] _oldAdjacentRegions = [];

	private void UpdateResource()
	{
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

	public SMRegion GetModel()
	{
		if (_model == null)
		{
			RebuildModel();
		}
		return _model;
	}
}
