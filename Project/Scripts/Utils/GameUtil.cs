using System;
using System.Collections.Generic;

namespace Smallworld.Models;

public partial class Game
{
    public static Game GetPresetGame(IServiceProvider serviceProvider, int numPlayers = 3, List<RacePower> initialRacePowers = null)
    {
        var game = new Game(serviceProvider);

        for (int i = 0; i < numPlayers; i++)
        {
            game.AddPlayer(new Player($"Player {i + 1}"));
        }

        game.SetRegions(GetPresetRegions());

        if (initialRacePowers == null)
        {
            initialRacePowers = new();
            for (int i = 0; i < 7; i++)
            {
                initialRacePowers.Add(game.GenerateNewRacePower());
            }
        }

        game.SetAvailableRacePowers(initialRacePowers);

        return game;
    }

    private static List<Region> GetPresetRegions()
    {
        // https://victoryconditions.com/wp-content/uploads/2020/01/small-world-four-player-map.jpg
        // Smallworld numbered regions - 1.png

        var regions = new List<Region> {
            // Seas and Lakes
            new(RegionType.Sea, RegionAttribute.None, true) { Name = "Northwest Sea" }, // 0
            new(RegionType.Sea, RegionAttribute.None, true) { Name = "Southeast Sea" }, // 1
            new(RegionType.Lake, RegionAttribute.None, false) { Name = "Central Lake" }, // 2

            // Borders
            new(RegionType.Forest, RegionAttribute.Underworld, true) { Name = "North Forest" }, // 3
            new(RegionType.Hill, RegionAttribute.None, true) { Name = "North Hills" }, // 4
            new(RegionType.Swamp, RegionAttribute.Magic, true) { Name = "Northeast Swamp" }, // 5
            new(RegionType.Forest, RegionAttribute.Underworld, true) { Name = "East Forest" }, // 6
            new(RegionType.Mountain, RegionAttribute.None, true) { Name = "East Mountains" }, // 7
            new(RegionType.Farmland, RegionAttribute.None, true) { Name = "East Farmlands" }, // 8
            new(RegionType.Swamp, RegionAttribute.Underworld, true) { Name = "Southeast Swamp" }, // 9
            new(RegionType.Hill, RegionAttribute.Magic, true) { Name = "Southeast Hills" }, // 10
            new(RegionType.Forest, RegionAttribute.None, true) { Name = "South Forest" }, // 11
            new(RegionType.Mountain, RegionAttribute.Mine, true) { Name = "Southeast Mountains" }, // 12
            new(RegionType.Farmland, RegionAttribute.None, true) { Name = "South Farmlands" }, // 13
            new(RegionType.Hill, RegionAttribute.Mine, true) { Name = "South Hills" }, // 14
            new(RegionType.Mountain, RegionAttribute.None, true) { Name = "Southwest Mountains" }, // 15
            new(RegionType.Farmland, RegionAttribute.None, true) { Name = "Southwest Farmlands" }, // 16
            new(RegionType.Swamp, RegionAttribute.Mine, true) { Name = "Southwest Swamp" }, // 17
            new(RegionType.Mountain, RegionAttribute.None, true) { Name = "West Mountains" }, // 18

            // Inner regions
            new(RegionType.Farmland, RegionAttribute.None, false) { Name = "Northwest Farmlands" }, // 19
            new(RegionType.Farmland, RegionAttribute.Mine, false) { Name = "North Farmlands" }, // 20
            new(RegionType.Farmland, RegionAttribute.None, false) { Name = "North Central Farmlands" }, // 21
            new(RegionType.Mountain, RegionAttribute.Mine, false) { Name = "North Central Mountains" }, // 22
            new(RegionType.Forest, RegionAttribute.Magic, false) { Name = "North Central Forest" }, // 23
            new(RegionType.Swamp, RegionAttribute.Mine, false) { Name = "East Central Swamp" }, // 24
            new(RegionType.Hill, RegionAttribute.None, false) { Name = "East Central Hills" },  // 25
            new(RegionType.Mountain, RegionAttribute.Underworld, false) { Name = "Southeast Central Mountains" }, // 26
            new(RegionType.Farmland, RegionAttribute.None, false) { Name = "South Central Farmlands" }, // 27
            new(RegionType.Swamp, RegionAttribute.Underworld, false) { Name = "South Central Swamp" }, // 28
            new(RegionType.Mountain, RegionAttribute.None, false) { Name = "South Central Mountains" }, // 29
            new(RegionType.Forest, RegionAttribute.Magic, false) { Name = "Southwest Central Forest" }, // 30
            new(RegionType.Mountain, RegionAttribute.Mine, false, RegionAttribute.Underworld) { Name = "West Central Mountains" }, // 31
            new(RegionType.Hill, RegionAttribute.None, false) { Name = "West Central Hills" }, // 32
            new(RegionType.Forest, RegionAttribute.None, false) { Name = "West Forest" }, // 33
            new(RegionType.Swamp, RegionAttribute.Magic, false) { Name = "West Swamp" }, // 34
            new(RegionType.Hill, RegionAttribute.Underworld, false ) { Name = "West Hills" }, // 35
            new(RegionType.Swamp, RegionAttribute.None, false) { Name = "Central Swamp" }, // 36
            new(RegionType.Hill, RegionAttribute.Magic, false) { Name = "Central Hills" }, // 37
            new(RegionType.Forest, RegionAttribute.Magic, false) { Name = "South Central Forest" } // 38
        };

        SetRegionAdjacentTo(regions, 0, new() { 3, 18, 19, 20, 33, 34, 35 });
        SetRegionAdjacentTo(regions, 1, new() { 10, 11 });
        SetRegionAdjacentTo(regions, 2, new() { 21, 22, 23, 24, 27, 29, 31, 37 });
        SetRegionAdjacentTo(regions, 3, new() { 0, 4, 20 });
        SetRegionAdjacentTo(regions, 4, new() { 3, 5, 20, 21, 22 });
        SetRegionAdjacentTo(regions, 5, new() { 4, 6, 22 });
        SetRegionAdjacentTo(regions, 6, new() { 5, 7, 22, 23 });
        SetRegionAdjacentTo(regions, 7, new() { 6, 8, 23, 24 });
        SetRegionAdjacentTo(regions, 8, new() { 7, 9, 24, 25 });
        SetRegionAdjacentTo(regions, 9, new() { 8, 10, 25 });
        SetRegionAdjacentTo(regions, 10, new() { 1, 9, 11, 25, 26 });
        SetRegionAdjacentTo(regions, 11, new() { 1, 10, 12, 26 });
        SetRegionAdjacentTo(regions, 12, new() { 11, 13, 26, 27, 38 });
        SetRegionAdjacentTo(regions, 13, new() { 12, 14, 28, 38 });
        SetRegionAdjacentTo(regions, 14, new() { 13, 15, 28 });
        SetRegionAdjacentTo(regions, 15, new() { 14, 16, 28, 30 });
        SetRegionAdjacentTo(regions, 16, new() { 15, 17, 30 });
        SetRegionAdjacentTo(regions, 17, new() { 16, 18, 30, 32 });
        SetRegionAdjacentTo(regions, 18, new() { 0, 17, 32, 33 });
        SetRegionAdjacentTo(regions, 19, new() { 0, 34 });
        SetRegionAdjacentTo(regions, 20, new() { 0, 3, 4, 21, 35 });
        SetRegionAdjacentTo(regions, 21, new() { 2, 4, 20, 22, 35, 36, 37 });
        SetRegionAdjacentTo(regions, 22, new() { 2, 4, 5, 6, 21, 23 });
        SetRegionAdjacentTo(regions, 23, new() { 2, 6, 7, 22, 24 });
        SetRegionAdjacentTo(regions, 24, new() { 2, 7, 8, 23, 25, 27 });
        SetRegionAdjacentTo(regions, 25, new() { 8, 9, 10, 24, 26, 27 });
        SetRegionAdjacentTo(regions, 26, new() { 10, 11, 12, 25, 27 });
        SetRegionAdjacentTo(regions, 27, new() { 2, 12, 24, 25, 26, 28, 29, 38 });
        SetRegionAdjacentTo(regions, 28, new() { 13, 14, 15, 27, 29, 30, 38 });
        SetRegionAdjacentTo(regions, 29, new() { 2, 27, 28, 30, 31 });
        SetRegionAdjacentTo(regions, 30, new() { 15, 16, 17, 28, 29, 31, 32 });
        SetRegionAdjacentTo(regions, 31, new() { 2, 29, 30, 32, 36, 37 });
        SetRegionAdjacentTo(regions, 32, new() { 17, 18, 30, 31, 33, 36 });
        SetRegionAdjacentTo(regions, 33, new() { 0, 18, 32, 34, 35, 36 });
        SetRegionAdjacentTo(regions, 34, new() { 0, 19, 33, 35 });
        SetRegionAdjacentTo(regions, 35, new() { 0, 20, 21, 33, 34, 36 });
        SetRegionAdjacentTo(regions, 36, new() { 21, 31, 32, 33, 35, 37 });
        SetRegionAdjacentTo(regions, 37, new() { 2, 21, 31, 36 });
        SetRegionAdjacentTo(regions, 38, new() { 12, 13, 27, 28 });

        AddLostTribeToRegion(regions, 3);
        AddLostTribeToRegion(regions, 17);
        AddLostTribeToRegion(regions, 20);
        AddLostTribeToRegion(regions, 28);
        AddLostTribeToRegion(regions, 30);
        AddLostTribeToRegion(regions, 38);

        return regions;
    }

    private static void SetRegionAdjacentTo(List<Region> regions, int regionIndex, List<int> adjacentRegionIndices)
    {
        regions[regionIndex].SetAdjacentRegions(adjacentRegionIndices.ConvertAll(index => regions[index]));
    }

    private static void AddLostTribeToRegion(List<Region> regions, int regionIndex)
    {
        regions[regionIndex].AddToken(Token.LostTribe);
    }
}