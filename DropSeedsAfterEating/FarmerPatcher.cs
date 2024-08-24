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
        if (Monitor != null)
        {
            Monitor.Log("EATS!");
        }

    }
}