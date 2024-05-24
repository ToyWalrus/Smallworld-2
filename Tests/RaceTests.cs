using Smallworld.Models;
using Smallworld.Models.Races;
using Smallworld.Models.Powers;

namespace Tests;

[TestClass]
public class RaceTests
{
    [TestMethod]
    public void Race_GetRedeploymentTokens_ReturnsTotalRaceTokensMinusOwnedRegions()
    {
        var race = new Amazon();
        var racepower = new RacePower(race, new Wealthy());

        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);

        region1.Conquer(racepower, 3); // 2
        region2.Conquer(racepower, 5); // 4

        Assert.AreEqual(race.GetRedeploymentTokens([region1, region2]).Count, 6);
    }

    [TestMethod]
    public void Dwarf_TallyRaceBonusVP_ReturnsNumberOfMinesOwnedEvenWhenInDecline()
    {
        var dwarf = new Dwarf();

        var mineRegion1 = new Region(RegionType.Farmland, RegionAttribute.Mine, false);
        var mineRegion2 = new Region(RegionType.Farmland, RegionAttribute.Mine, false);
        var otherRegion1 = new Region(RegionType.Mountain, RegionAttribute.None, false);
        var otherRegion2 = new Region(RegionType.Swamp, RegionAttribute.None, false);
        var regions = new List<Region> { mineRegion1, mineRegion2, otherRegion1, otherRegion2 };

        Assert.AreEqual(dwarf.TallyRaceBonusVP(regions), 2);

        dwarf.EnterDecline();

        Assert.AreEqual(dwarf.TallyRaceBonusVP(regions), 2);
    }

    [TestMethod]
    public void Giant_GetRegionConquerCostReduction_ReturnsOneIfAdjacentToMountain()
    {
        var giant = new Giant();
        var farmland = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var mountain = new Region(RegionType.Mountain, RegionAttribute.None, false);

        farmland.SetAdjacentRegions([mountain]);
        mountain.SetAdjacentRegions([farmland]);

        Assert.AreEqual(giant.GetRegionConquerCostReduction(farmland), 1);
        Assert.AreEqual(giant.GetRegionConquerCostReduction(mountain), 0);
    }

    [TestMethod]
    public void Halfling_GetInvalidConquerReasons_ReturnsTrueIfRegionIsNotBorderAndIsTheFirstConquest()
    {
        var halfling = new Halfling();
        var region = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var unconnectedBorderRegion = new Region(RegionType.Farmland, RegionAttribute.None, true);

        Assert.AreEqual(halfling.GetInvalidConquerReasons([], region, true).Count, 0);
        Assert.AreNotEqual(halfling.GetInvalidConquerReasons([region], unconnectedBorderRegion, false).Count, 0); // Not first conquest, not connected
    }

    [TestMethod]
    public void Halfling_OnRegionConquered_AddsHoleInTheGroundTokenToRegionIfLessThanTwoRegionsConquered()
    {
        var halfling = new Halfling();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region3 = new Region(RegionType.Farmland, RegionAttribute.None, false);

        Assert.IsFalse(region1.HasToken(Token.HoleInTheGround));
        Assert.IsFalse(region2.HasToken(Token.HoleInTheGround));
        Assert.IsFalse(region3.HasToken(Token.HoleInTheGround));

        halfling.OnRegionConquered(region1);
        halfling.OnRegionConquered(region2);
        halfling.OnRegionConquered(region3);

        Assert.IsTrue(region1.HasToken(Token.HoleInTheGround));
        Assert.IsTrue(region2.HasToken(Token.HoleInTheGround));
        Assert.IsFalse(region3.HasToken(Token.HoleInTheGround));
    }

    [TestMethod]
    public void Human_TallyRaceBonusVP_ReturnsNumberOfFarmlandRegionsOwnedWhenNotInDecline()
    {
        var human = new Human();

        var farmlandRegion1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var farmlandRegion2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var otherRegion1 = new Region(RegionType.Mountain, RegionAttribute.None, false);
        var otherRegion2 = new Region(RegionType.Swamp, RegionAttribute.None, false);
        var regions = new List<Region> { farmlandRegion1, farmlandRegion2, otherRegion1, otherRegion2 };

        Assert.AreEqual(human.TallyRaceBonusVP(regions), 2);

        human.EnterDecline();

        Assert.AreEqual(human.TallyRaceBonusVP(regions), 0);
    }

    [TestMethod]
    public void Orc_TallyRaceBonusVP_ReturnsNumberOfNonEmptyRegionsConqueredThisTurn()
    {
        var orc = new Orc();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var emptyRegion = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var regions = new List<Region> { region1, region2, emptyRegion };

        region1.AddToken(Token.LostTribe);
        region2.AddToken(Token.LostTribe);

        orc.OnTurnStart();
        orc.OnRegionConquered(region1);
        orc.OnRegionConquered(region2);
        orc.OnRegionConquered(emptyRegion);

        Assert.AreEqual(orc.TallyRaceBonusVP(regions), 2);

        orc.OnTurnStart();

        // Should only count non-empty regions conquered this turn
        Assert.AreEqual(orc.TallyRaceBonusVP(regions), 0);
    }

    [TestMethod]
    public void Skeleton_GetRedeploymentTokens_ReturnsExtraTroopsBasedOnNonEmptyRegionsConqueredThisTurn()
    {
        var skeleton = new Skeleton();
        var rp = new RacePower(skeleton, new Wealthy());

        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var emptyRegion = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var regions = new List<Region> { region1, region2, emptyRegion };

        region1.AddToken(Token.LostTribe);
        region2.AddToken(Token.LostTribe);

        skeleton.OnTurnStart();

        region1.Conquer(rp, 3);
        skeleton.OnRegionConquered(region1);

        region2.Conquer(rp, 3);
        skeleton.OnRegionConquered(region2);

        emptyRegion.Conquer(rp, 3);
        skeleton.OnRegionConquered(emptyRegion);

        // Each region will give 2 token as base (6 total)
        // +1/2 for each non-empty region conquered this turn (2 regions, 1 extra token)
        Assert.AreEqual(skeleton.GetRedeploymentTokens(regions).Count, 7);

        skeleton.OnTurnStart();

        // No regions conquered this turn, will only get base redeployment
        Assert.AreEqual(skeleton.GetRedeploymentTokens(regions).Count, 6);
    }

    [TestMethod]
    public void Triton_GetRegionConquerCostReduction_ReturnsOneIfAdjacentToSeaOrLake()
    {
        var triton = new Triton();

        var regionNextToSea = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var regionNextToLake = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var sea = new Region(RegionType.Sea, RegionAttribute.None, false);
        var lake = new Region(RegionType.Lake, RegionAttribute.None, false);

        regionNextToSea.SetAdjacentRegions([sea]);
        regionNextToLake.SetAdjacentRegions([lake]);
        sea.SetAdjacentRegions([regionNextToSea]);
        lake.SetAdjacentRegions([regionNextToLake]);

        Assert.AreEqual(triton.GetRegionConquerCostReduction(regionNextToSea), 1);
        Assert.AreEqual(triton.GetRegionConquerCostReduction(regionNextToLake), 1);
        Assert.AreEqual(triton.GetRegionConquerCostReduction(sea), 0);
        Assert.AreEqual(triton.GetRegionConquerCostReduction(lake), 0);
    }

    [TestMethod]
    public void Troll_OnRegionConquered_AddsTrollLairTokenToRegion()
    {
        var troll = new Troll();
        var region = new Region(RegionType.Farmland, RegionAttribute.None, false);

        Assert.IsFalse(region.HasToken(Token.TrollLair));

        troll.OnRegionConquered(region);

        Assert.IsTrue(region.HasToken(Token.TrollLair));
    }

    [TestMethod]
    public void Wizard_TallyRaceBonusVP_ReturnsNumberOfMagicRegionsOwnedWhenNotInDecline()
    {
        var wizard = new Wizard();

        var magicRegion1 = new Region(RegionType.Farmland, RegionAttribute.Magic, false);
        var magicRegion2 = new Region(RegionType.Farmland, RegionAttribute.Magic, false);
        var otherRegion1 = new Region(RegionType.Mountain, RegionAttribute.None, false);
        var otherRegion2 = new Region(RegionType.Swamp, RegionAttribute.None, false);
        var regions = new List<Region> { magicRegion1, magicRegion2, otherRegion1, otherRegion2 };

        Assert.AreEqual(wizard.TallyRaceBonusVP(regions), 2);

        wizard.EnterDecline();

        Assert.AreEqual(wizard.TallyRaceBonusVP(regions), 0);
    }
}
