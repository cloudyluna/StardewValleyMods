namespace DropSeedsAfterEating;

using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;

internal class FarmerPatcher
{
    private static IMonitor? Monitor;

    internal static void Initialize(IMonitor monitor)
    {
        Monitor = monitor;
    }

    internal static bool eatObject_Prefix(Farmer __instance, Object o, bool overrideFullness, out object? __state)
    {
        __state = null;

        return true;

    }

    internal static void eatObject_Postfix(Farmer __instance, Object o, bool overrideFullness, in object? __state)
    {
        if (Monitor != null && o != null)
        {
            var food = (Item)o;
            var ediblePlants = food.Category == Object.GreensCategory || food.Category == Object.VegetableCategory;
            if (ediblePlants)
            {
                foreach (KeyValuePair<string, CropData> cropDatum in Game1.cropData)
                {
                    if (ItemRegistry.HasItemId(food, cropDatum.Value.HarvestItemId))
                    {
                        var seedId = cropDatum.Key;
                        var seed = ItemRegistry.Create(seedId);

                        Monitor.Log($"Dropping a seed of {seed.Name} for: {food.Name}");
                        Game1.player.addItemToInventory(seed);

                        break;
                    }
                }
            }
        }

    }
}