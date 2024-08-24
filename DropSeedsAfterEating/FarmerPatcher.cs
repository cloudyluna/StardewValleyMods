namespace DropSeedsAfterEating;

using StardewModdingAPI;
using StardewValley;

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

            if (food.Category == Object.FruitsCategory
                || food.Category == Object.GreensCategory
                || food.Category == Object.VegetableCategory
                )
            {
                if (food.Name.Equals("Banana"))
                {
                    var seed = ItemRegistry.ResolveMetadata("(O)69")?.CreateItem();
                    if (seed != null)
                    {
                        Monitor.Log($"Dropping seeds for: {food.Name}");

                        Game1.player.addItemToInventory(seed);
                    }
                }
            }
        }

    }
}