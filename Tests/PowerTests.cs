using Microsoft.Extensions.DependencyInjection;
using Smallworld.IO;
using Smallworld.Models;
using Smallworld.Models.Powers;

namespace Tests;

[TestClass]
public class PowerTests
{

    static private readonly RollDiceMock rollDiceMock = new(0);
    static private readonly ConfirmationMock confirmationMock = new();
    static private IModelFactory<Power> pFactory = null!;

    [ClassInitialize]
    static public void Init(TestContext _)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddTransient<IConfirmation, ConfirmationMock>(_ => confirmationMock)
            .AddTransient<IRollDice, RollDiceMock>(_ => rollDiceMock)
            .AddTransient<ISelection<Player>, SelectionMock<Player>>(_ => new SelectionMock<Player>(new Player("test")))
            .AddTransient<IModelFactory<Power>, PowerFactory>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        pFactory = serviceProvider.GetService<IModelFactory<Power>>()!;
    }

    [TestMethod]
    public void Alchemist_TallyPowerBonusVP_ReturnsTwo()
    {
        var alchemist = pFactory.Create<Alchemist>();

        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region3 = new Region(RegionType.Farmland, RegionAttribute.None, false);

        Assert.AreEqual(alchemist.TallyPowerBonusVP([region1, region2, region3]), 2);
        Assert.AreEqual(alchemist.TallyPowerBonusVP([]), 2);

        alchemist.EnterDecline();

        Assert.AreEqual(alchemist.TallyPowerBonusVP([region1, region2, region3]), 0);
    }

    [TestMethod]
    public void Berserk_GetRegionConquerCostReduction_ReturnsTheRollOfTheDie()
    {
        var berserk = pFactory.Create<Berserk>();
        var region = new Region(RegionType.Farmland, RegionAttribute.None, false);

        rollDiceMock.SetValue(0);
        Assert.AreEqual(berserk.GetRegionConquerCostReduction(region).Result, 0);

        rollDiceMock.SetValue(1);
        Assert.AreEqual(berserk.GetRegionConquerCostReduction(region).Result, 1);

        rollDiceMock.SetValue(3);
        Assert.AreEqual(berserk.GetRegionConquerCostReduction(region).Result, 3);
    }

    [TestMethod]
    public void Bivouacking_GetRedeploymentTokens_Returns5EncampmentTokensAndRemovesEncampmentsFromOwnedRegions()
    {
        var bivouacking = pFactory.Create<Bivouacking>();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region1.AddToken(Token.Encampment);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region2.AddToken(Token.Encampment);
        var region3 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var regions = new List<Region> { region1, region2, region3 };

        var tokens = bivouacking.GetRedeploymentTokens(regions);

        Assert.AreEqual(tokens.Count, 5);
        Assert.IsTrue(tokens.TrueForAll(token => token == Token.Encampment));
        Assert.IsFalse(region1.HasToken(Token.Encampment));
        Assert.IsFalse(region2.HasToken(Token.Encampment));
    }

    [TestMethod]
    public void Commando_GetRegionConquerCostReduction_Returns1()
    {
        var commando = pFactory.Create<Commando>();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Swamp, RegionAttribute.Magic, true);

        Assert.AreEqual(commando.GetRegionConquerCostReduction(region1).Result, 1);
        Assert.AreEqual(commando.GetRegionConquerCostReduction(region2).Result, 1);
    }

    // TODO: come back when player-picking is implemented
    [TestMethod, Ignore]
    public void Diplomat_OnTurnEnd_PromptsPlayerToPickAnotherPlayerNotAttackedThisTurn()
    {
    }

    [TestMethod]
    public void DragonMaster_GetRegionConquerCostReduction_ReturnsVeryHighNumberWhenChosingToPlaceDragon()
    {
        var dragonMaster = pFactory.Create<DragonMaster>();
        var region = new Region(RegionType.Farmland, RegionAttribute.None, false);

        confirmationMock.SetShouldConfirm(true);

        Assert.AreEqual(dragonMaster.GetRegionConquerCostReduction(region).Result, int.MaxValue);
    }

    [TestMethod]
    public void DragonMaster_GetRegionConquerCostReduction_Returns0IfNotConfirmed()
    {
        var dragonMaster = pFactory.Create<DragonMaster>();
        var region = new Region(RegionType.Farmland, RegionAttribute.None, false);

        confirmationMock.SetShouldConfirm(false);

        Assert.AreEqual(dragonMaster.GetRegionConquerCostReduction(region).Result, 0);
    }

    [TestMethod]
    public async void DragonMaster_GetRegionConquerCostReduction_Returns0IfAlreadyUsedDragon()
    {
        var dragonMaster = pFactory.Create<DragonMaster>();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);

        // use the dragon token
        await dragonMaster.GetRegionConquerCostReduction(region1);

        Assert.AreEqual(dragonMaster.GetRegionConquerCostReduction(region2).Result, 0);
    }

    [TestMethod]
    public void Flying_GetInvalidConquerReasons_ReturnsReasonsWithoutBorderOrAdjacentRestriction()
    {
        var flying = pFactory.Create<Flying>();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Mountain, RegionAttribute.None, false);
        var region3 = new Region(RegionType.Sea, RegionAttribute.None, true);
        var region4 = new Region(RegionType.Lake, RegionAttribute.None, false);

        Assert.AreEqual(flying.GetInvalidConquerReasons([region1], region1).Count, 0);
        Assert.AreEqual(flying.GetInvalidConquerReasons([region1], region2).Count, 0);
        Assert.AreNotEqual(flying.GetInvalidConquerReasons([region1], region3).Count, 0); // Cannot conquer sea regions
        Assert.AreNotEqual(flying.GetInvalidConquerReasons([region1], region4).Count, 0); // Cannot conquer lake regions
    }

    [TestMethod]
    public void Forest_TallyPowerBonusVP_ReturnsOnePerForestRegionWhenNotInDecline()
    {
        var forest = pFactory.Create<Forest>();
        var region1 = new Region(RegionType.Forest, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Forest, RegionAttribute.None, false);
        var region3 = new Region(RegionType.Swamp, RegionAttribute.None, false);
        var regions = new List<Region> { region1, region2, region3 };

        Assert.AreEqual(forest.TallyPowerBonusVP(regions), 2);

        forest.EnterDecline();

        Assert.AreEqual(forest.TallyPowerBonusVP(regions), 0);
    }

    [TestMethod]
    public void Fortified_GetRedeploymentTokens_Returns1FortressTokenIfLessThanMaxFortsBuilt()
    {
        var fortified = pFactory.Create<Fortified>();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region1.AddToken(Token.Fortress);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var regions = new List<Region> { region1, region2 };

        var tokens = fortified.GetRedeploymentTokens(regions);

        Assert.AreEqual(tokens.Count, 1);
        Assert.AreEqual(tokens[0], Token.Fortress);
    }

    [TestMethod]
    public void Fortified_GetRedeploymentTokens_ReturnsNoneIfAllRegionsHaveFortressToken()
    {
        var fortified = pFactory.Create<Fortified>();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region1.AddToken(Token.Fortress);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region2.AddToken(Token.Fortress);

        Assert.AreEqual(fortified.GetRedeploymentTokens([region1, region2]).Count, 0);
    }

    [TestMethod]
    public void Fortified_GetRedeploymentTokens_ReturnsNoneIfMaxFortsBuilt()
    {
        var fortified = pFactory.Create<Fortified>();
        var region = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region.AddToken(Token.Fortress);

        Assert.AreEqual(fortified.GetRedeploymentTokens(Enumerable.Repeat(region, 6).ToList()).Count, 0);
    }

    [TestMethod]
    public void Fortified_TallyPowerBonusVP_ReturnsNumberOfFortressTokensOwnedWhenNotInDecline()
    {
        var fortified = pFactory.Create<Fortified>();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region1.AddToken(Token.Fortress);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region2.AddToken(Token.Fortress);
        var region3 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var regions = new List<Region> { region1, region2, region3 };

        Assert.AreEqual(fortified.TallyPowerBonusVP(regions), 2);

        fortified.EnterDecline();

        Assert.AreEqual(fortified.TallyPowerBonusVP(regions), 0);
    }

    [TestMethod]
    public void Heroic_GetRedeploymentTokens_ReturnsTwoHeroicTokensAndRemovesAllHeroicTokensFromOwnedRegions()
    {
        var heroic = pFactory.Create<Heroic>();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region1.AddToken(Token.Heroic);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region2.AddToken(Token.Heroic);

        var tokens = heroic.GetRedeploymentTokens([region1, region2]);

        Assert.AreEqual(tokens.Count, 2);
        Assert.IsTrue(tokens.TrueForAll(token => token == Token.Heroic));
        Assert.IsFalse(region1.HasToken(Token.Heroic));
        Assert.IsFalse(region2.HasToken(Token.Heroic));
    }

    [TestMethod]
    public void Hill_TallyPowerBonusVP_ReturnsOnePerHillRegionWhenNotInDecline()
    {
        var hill = pFactory.Create<Hill>();
        var region1 = new Region(RegionType.Hill, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Hill, RegionAttribute.None, false);
        var region3 = new Region(RegionType.Swamp, RegionAttribute.None, false);
        var regions = new List<Region> { region1, region2, region3 };

        Assert.AreEqual(hill.TallyPowerBonusVP(regions), 2);

        hill.EnterDecline();

        Assert.AreEqual(hill.TallyPowerBonusVP(regions), 0);
    }

    [TestMethod]
    public void Merchant_TallyPowerBonusVP_ReturnsOnePerRegionOwnedWhenNotInDecline()
    {
        var merchant = pFactory.Create<Merchant>();
        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Forest, RegionAttribute.Magic, false);
        var region3 = new Region(RegionType.Swamp, RegionAttribute.Underworld, true);
        var regions = new List<Region> { region1, region2, region3 };

        Assert.AreEqual(merchant.TallyPowerBonusVP(regions), 3);

        merchant.EnterDecline();

        Assert.AreEqual(merchant.TallyPowerBonusVP(regions), 0);
    }

    [TestMethod]
    public void Mounted_GetRegionConquerCostReduction_Returns1ForHillsAndFarmland()
    {
        var mounted = pFactory.Create<Mounted>();
        var farmlandRegion = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var hillRegion = new Region(RegionType.Hill, RegionAttribute.None, true);
        var otherRegion = new Region(RegionType.Swamp, RegionAttribute.None, false);

        Assert.AreEqual(mounted.GetRegionConquerCostReduction(farmlandRegion).Result, 1);
        Assert.AreEqual(mounted.GetRegionConquerCostReduction(hillRegion).Result, 1);
        Assert.AreEqual(mounted.GetRegionConquerCostReduction(otherRegion).Result, 0);
    }

    [TestMethod]
    public void Pillaging_TallyPowerBonusVP_ReturnsOnePerOccupiedRegionConqueredThisTurn()
    {
        var pillaging = pFactory.Create<Pillaging>();

        var region1 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region1.AddToken(Token.LostTribe);
        var region2 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        region2.AddToken(Token.LostTribe);
        var region3 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var list = new List<Region> { region1, region2, region3 };

        pillaging.OnRegionConquered(region1);
        pillaging.OnRegionConquered(region2);
        pillaging.OnRegionConquered(region3);

        Assert.AreEqual(pillaging.TallyPowerBonusVP(list), 2);

        pillaging.OnTurnStart();

        Assert.AreEqual(pillaging.TallyPowerBonusVP(list), 0);
    }

    [TestMethod]
    public void Seafaring_GetInvalidConquerReasons_ReturnsReasonsWithoutSeaOrLakeRestriction()
    {
        var seafaring = pFactory.Create<Seafaring>();
        var region1 = new Region(RegionType.Sea, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Lake, RegionAttribute.None, false);
        var region3 = new Region(RegionType.Farmland, RegionAttribute.None, false);
        var region4 = new Region(RegionType.Mountain, RegionAttribute.None, false);
        var ownedRegions = new List<Region> { region1, region4 };

        region1.SetAdjacentRegions([region2, region3, region4]);

        Assert.IsFalse(seafaring.GetInvalidConquerReasons(ownedRegions, region1).Contains(InvalidConquerReason.SeaOrLake));
        Assert.IsFalse(seafaring.GetInvalidConquerReasons(ownedRegions, region2).Contains(InvalidConquerReason.SeaOrLake));
    }

    [TestMethod]
    public async void Stout_OnTurnEnd_CanEnterDecline()
    {
        var stout = pFactory.Create<Stout>();

        confirmationMock.SetShouldConfirm(true);

        Assert.IsFalse(stout.IsInDecline);

        await stout.OnTurnEnd();

        Assert.IsTrue(stout.IsInDecline);
    }

    [TestMethod]
    public async void Stout_OnTurnEnd_CanChooseNotToEnterDecline()
    {
        var stout = pFactory.Create<Stout>();

        confirmationMock.SetShouldConfirm(false);

        Assert.IsFalse(stout.IsInDecline);

        await stout.OnTurnEnd();

        Assert.IsFalse(stout.IsInDecline);
    }

    [TestMethod]
    public void Swamp_TallyPowerBonusVP_ReturnsOnePerSwampRegionWhenNotInDecline()
    {
        var swamp = pFactory.Create<Swamp>();
        var region1 = new Region(RegionType.Swamp, RegionAttribute.None, false);
        var region2 = new Region(RegionType.Swamp, RegionAttribute.None, false);
        var region3 = new Region(RegionType.Forest, RegionAttribute.None, false);
        var regions = new List<Region> { region1, region2, region3 };

        Assert.AreEqual(swamp.TallyPowerBonusVP(regions), 2);

        swamp.EnterDecline();

        Assert.AreEqual(swamp.TallyPowerBonusVP(regions), 0);
    }

    [TestMethod]
    public void Underworld_GetRegionConquerCostReduction_Returns1ForUnderworldRegions()
    {
        var underworld = pFactory.Create<Underworld>();
        var underworldRegion = new Region(RegionType.Farmland, RegionAttribute.Underworld, false);
        var otherRegion = new Region(RegionType.Farmland, RegionAttribute.None, false);

        Assert.AreEqual(underworld.GetRegionConquerCostReduction(underworldRegion).Result, 1);
        Assert.AreEqual(underworld.GetRegionConquerCostReduction(otherRegion).Result, 0);
    }

    [TestMethod]
    public void Wealthy_TallyPowerBonusVP_Returns7Once()
    {
        var wealthy = pFactory.Create<Wealthy>();

        Assert.AreEqual(wealthy.TallyPowerBonusVP([]), 7);
        Assert.AreEqual(wealthy.TallyPowerBonusVP([]), 0);
        Assert.AreEqual(wealthy.TallyPowerBonusVP([]), 0);
    }
}
