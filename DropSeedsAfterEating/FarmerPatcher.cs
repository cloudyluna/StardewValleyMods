namespace DropSeedsAfterEating;

using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;

enum FoodQuality
{
    Normal = 0,
    Silver = 1,
    Gold = 2,
    Iridium = 4,
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

enum FoodPriceLevel
{
    Cheap,
    Affordable,
    Expensive,
    Premium,
}

internal class FarmerPatcher
{
    private const int defaultHowManyToDrop = 1;

    private static IMonitor? Monitor;

    internal static void Initialize(IMonitor monitor)
    {
        Monitor = monitor;
    }

    internal static void EatObject_Postfix(Farmer __instance, Object o, bool overrideFullness)
    {
        var farmer = __instance;
        try
        {
            Item? eatenFood = o;
            if (eatenFood != null)
            {
                // TODO: Do we want to include vanilla saplings?
                // There's no metadata for fruit->seeds, I believe.
                // This means we probably have to hardcode our own
                // list of fruit->seed manually if we want to
                // proceed through.
                var isAConsumablePlant =
                    eatenFood.Category == Object.FruitsCategory // These are not (big, like an apple) tree fruits!
                    || eatenFood.Category == Object.GreensCategory
                    || eatenFood.Category == Object.VegetableCategory
                    || eatenFood.Category == Object.flowersCategory;

                // TODO: Add config key.
                double chanceModifier = 1.0;
                var canDropSeed = CanDropSeedIfLucky(
                    eatenFood,
                    farmer,
                    chanceModifier,
                    out int howManyToDrop
                );

                if (isAConsumablePlant && canDropSeed)
                {
                    DropSeedFrom(eatenFood, howManyToDrop);
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

    private static void DropSeedFrom(Item eatenFood, int howManyToDrop)
    {
        foreach (KeyValuePair<string, CropData> datum in Game1.cropData)
        {
            if (ItemRegistry.HasItemId(eatenFood, datum.Value.HarvestItemId))
            {
                var seedId = datum.Key;
                var seed = ItemRegistry.Create(seedId, amount: howManyToDrop);

                Monitor?.Log($"Dropping a seed of {seed.Name} for: {eatenFood.Name}");


                Item? seedThatDidntGetAdded = Game1.player.addItemToInventory(seed);
                if (seedThatDidntGetAdded != null)
                {
                    Game1.player.dropItem(seedThatDidntGetAdded);
                };

                break;
            }
        }
    }

    private static bool CanDropSeedIfLucky(
        Item food,
        Farmer farmer,
        double chanceModifier,
        out int howManyToDrop
    )
    {
        static double toPercentage(double luck)
        {
            var minLuck = -0.12;
            var maxLuck = 0.12;

            if (luck < minLuck || luck > maxLuck)
            {
                Monitor?.Log("Other mods that overrides the default vanilla luck value is not supported and considered incompatible!", LogLevel.Error);
                throw new InvalidOperationException($"The luck value {luck} is out of range");
            }

            var range = maxLuck - minLuck;

            return Math.Round((luck - minLuck) / range * 100.0);
        }

        var foodQuality = (FoodQuality)food.Quality;
        var foodPrice = food.sellToStorePrice(farmer.UniqueMultiplayerID);

        howManyToDrop = GetHowManyToDrop(foodPrice, foodQuality, farmer.DailyLuck);

        var luckPercentage = toPercentage(farmer.DailyLuck);

        return new Random().NextDouble() * 100.0 < luckPercentage;
    }

    private static int GetHowManyToDrop(int foodPrice, FoodQuality foodQuality, double luck)
    {
        var luckBonus = LuckValueToLuckLevel(luck) switch
        {
            LuckLevel.FeelingLucky => 5,
            LuckLevel.LuckyButNotTooLucky => 4,
            LuckLevel.NeutralGood => 3,
            LuckLevel.NeutralBad => 2,
            LuckLevel.NotFeelingLuckyAtAll => 1,
            LuckLevel.MaybeStayHome => 0,
            _ => defaultHowManyToDrop,
        };

        var qualityBonus = foodQuality switch
        {
            FoodQuality.Normal => 2,
            FoodQuality.Silver => 3,
            FoodQuality.Gold => 4,
            FoodQuality.Iridium => 5,
            _ => throw new InvalidDataException($"Impossible: value {foodQuality} is not recognized"),
        };

        var pricePenalty = FoodPriceValueToPriceLevel(foodPrice) switch
        {
            FoodPriceLevel.Cheap => 1,
            FoodPriceLevel.Affordable => new Random().Next(1, 2),
            FoodPriceLevel.Expensive => 3,
            FoodPriceLevel.Premium => new Random().Next(3, 4),
            _ => 2,
        };

        var random = new Random();
        var totalBonus = random.Next(luckBonus) + random.Next(qualityBonus);
        var dropAmount = totalBonus - pricePenalty;

        return (dropAmount < 1) ? defaultHowManyToDrop : dropAmount;
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

    private static FoodPriceLevel FoodPriceValueToPriceLevel(int price)
    {
        return price switch
        {
            var v when v < 300 => FoodPriceLevel.Cheap,
            var v when v >= 300 && v < 650 => FoodPriceLevel.Affordable,
            var v when v >= 650 && v <= 1000 => FoodPriceLevel.Expensive,
            var _ => FoodPriceLevel.Premium,

        };
    }
}