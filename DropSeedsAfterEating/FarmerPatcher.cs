namespace DropSeedsAfterEating;

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
        double minDailyLuck = -0.12;
        double maxDailyLuck = 0.12;
        howManyToDrop = defaultHowManyToDrop;
        return false;

    }

    private static int ofPercentage(int current, int max)
    {
        return 0;
    }
}