namespace DropSeedsAfterEating;

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
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

    private static IMonitor? Monitor;

    internal static void Initialize(IMonitor monitor)
    {
        Monitor = monitor;
    }

    internal static void eatObject_Postfix(Farmer __instance, Object o, bool overrideFullness)
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
                    || food.Category == Object.VegetableCategory;

                // TODO: Add config key.
                int minChance = 2;
                var canDropSeeds = canDropSeedsIfLucky(
                    (FoodQuality)food.Quality,
                    Game1.player.DailyLuck,
                    minChance,
                    out int howManyToDrop
                );

                if (isAConsumablePlant) // && canDropSeeds
                {
                    tryDroppingSeedsFrom(food, howManyToDrop);
                };
            }
        }
        catch (Exception ex)
        {
            Monitor?.Log($"Failed in patched code of: {nameof(eatObject_Postfix)}:\n{ex}", LogLevel.Error);
        }
    }

    private static void tryDroppingSeedsFrom(Item food, int howManyToDrop)
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

    private static bool canDropSeedsIfLucky(
        FoodQuality foodQuality,
        double todaysLuck,
        int minChance,
        out int howManyToDrop
    )
    {
        howManyToDrop = defaultHowManyToDrop;
        return false;

    }

    private static LuckLevel luckValueToLuckLevel(double luckValue)
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

    private static int ofPercentage(int current, int max)
    {
        return 0;
    }
}