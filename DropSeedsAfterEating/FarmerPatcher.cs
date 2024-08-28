namespace DropSeedsAfterEating;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Crops;
using StardewValley.Menus;

enum FoodQuality
{
    Normal,
    Silver,
    Gold,
    Iridium,
}

// These names are taken from UIInfoSuite2 so we can match them accordingly
// as it's a quite popular mod.
enum LuckLevel
{
    FeelingLucky,
    LuckyButNotTooLucky,
    NeutralGood,
    NeutralBad,
    NotFeelingLuckyAtAll,
    MaybeStayHome,
}

internal class FarmerPatcher
{
    private const int defaultHowManyToDrop = 1;

    private static Random? PersistentRandom;

    private static IMonitor? Monitor;

    internal static void Initialize(IMonitor monitor, Random randomSeed)
    {
        Monitor = monitor;
        PersistentRandom = randomSeed;
    }

    internal static void EatObject_Postfix(Farmer __instance, Object o, bool overrideFullness)
    {
        try
        {
            Item? food = o;
            if (food != null)
            {
                // TODO: Do we want to include vanilla saplings?
                // There's no metadata for fruit->seeds, I believe.
                // This means we probably have to hardcode our own
                // list of fruit->seed manually if we want to
                // proceed through.
                var isAConsumablePlant =
                    food.Category == Object.FruitsCategory // These are not (big, like an apple) tree fruits!
                    || food.Category == Object.GreensCategory
                    || food.Category == Object.VegetableCategory
                    || food.Category == Object.flowersCategory;

                // TODO: Add config key.
                double dailyLuckWeight = 1.0;
                double chanceModifier = 1.0;
                var canDropSeeds = CanDropSeedsIfLucky(
                    (FoodQuality)food.Quality,
                    Game1.player.DailyLuck,
                    dailyLuckWeight,
                    chanceModifier,
                    out int howManyToDrop
                );

                if (isAConsumablePlant && canDropSeeds)
                {
                    TryDroppingSeedsFrom(food, howManyToDrop);
                };
            }
        }
        catch (Exception ex)
        {
            Monitor?.Log(
                $"Failed in patched code of: {nameof(EatObject_Postfix)}:\n{ex}",
                LogLevel.Error
            );
        }
    }

    private static void TryDroppingSeedsFrom(Item food, int howManyToDrop)
    {
        foreach (KeyValuePair<string, CropData> datum in Game1.cropData)
        {
            if (ItemRegistry.HasItemId(food, datum.Value.HarvestItemId))
            {
                var seedId = datum.Key;
                var seed = ItemRegistry.Create(seedId, amount: howManyToDrop);

                Monitor?.Log($"Dropping a seed of {seed.Name} for: {food.Name}");


                Item? seedThatDidntGetAdded = Game1.player.addItemToInventory(seed);
                if (seedThatDidntGetAdded != null)
                {
                    Game1.player.dropItem(seedThatDidntGetAdded);
                };

                break;
            }
        }
    }

    private static bool CanDropSeedsIfLucky(
        FoodQuality foodQuality,
        double todaysLuck,
        double dailyLuckWeight,
        double chanceModifier,
        out int howManyToDrop
    )
    {
        howManyToDrop = GetHowManyToDrop(foodQuality, todaysLuck);
        double num = 1.0 * Game1.player.team.AverageDailyLuck() * dailyLuckWeight;
        double val = 50.0 * chanceModifier * num;
        var chance = Game1.random.NextBool(num + val);
        Monitor?.Log($"LUCK: ${val}");
        Monitor?.Log($"LUCKY?: ${chance}");
        return chance;
    }

    private static int GetHowManyToDrop(FoodQuality foodQuality, double luckValue)
    {
        var defaultChanceWeight = 0.025;
        double amountDropChance = foodQuality switch
        {
            FoodQuality.Normal => luckValue * defaultChanceWeight,
            FoodQuality.Silver => luckValue * (defaultChanceWeight * 2),
            FoodQuality.Gold => luckValue * (defaultChanceWeight * 3),
            FoodQuality.Iridium => luckValue * (defaultChanceWeight * 3),
            _ => defaultChanceWeight,
        };


        if (PersistentRandom != null)
        {
            var dropAmount = PersistentRandom.Next((int)amountDropChance);
            return (dropAmount < 1) ? defaultHowManyToDrop : dropAmount;
        }

        return defaultHowManyToDrop;

    }

    private static LuckLevel LuckValueToLuckLevel(double luckValue)
    {
        return luckValue switch
        {
            var v when v > 0.07 => LuckLevel.FeelingLucky,
            var v when v > 0.02 && v <= 0.07 => LuckLevel.LuckyButNotTooLucky,
            var v when v >= -0.02 && v <= 0.02 && v != 0 => LuckLevel.NeutralGood,
            var v when v == 0 => LuckLevel.NeutralBad,
            var v when v >= -0.07 && v < -0.02 => LuckLevel.NotFeelingLuckyAtAll,
            var v when v < -0.07 => LuckLevel.MaybeStayHome,
            var _ => LuckLevel.NeutralBad,
        };
    }

    private static int OfPercentage(double current, double max)
    {
        return (int)(current / max * 100.0);
    }
}