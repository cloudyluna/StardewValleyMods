namespace IdenticalIndoorGlowRingRadius;

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley.Locations;

internal class RingPatcher
{
    private static IMonitor? Monitor;

    internal static void Initialize(IMonitor monitor)
    {
        Monitor = monitor;
    }

    internal static bool Update_Prefix(Ring __instance, int? ____lightSourceID, GameTime time, GameLocation environment, Farmer who)
    {
        try
        {
            if (!____lightSourceID.HasValue)
            {
                return false;
            }
            Vector2 zero = Vector2.Zero;
            if (who.shouldShadowBeOffset)
            {
                zero += who.drawOffset;
            }

            environment.repositionLightSource(____lightSourceID.Value, new Vector2(who.Position.X + 21f, who.Position.Y) + zero);

            return false;

        }

        catch (Exception ex)
        {
            Monitor?.Log($"Failed in patched code: {nameof(Update_Prefix)}:\n{ex}", LogLevel.Error);
            return true;
        }
    }
}