namespace MaintainGlowRingsIndoorsRadius;

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI;

internal class RingPatcher
{
    private static IMonitor? Monitor;

    internal static void Initialize(IMonitor monitor)
    {
        Monitor = monitor;
    }

    internal static bool Update_Prefix(Ring __instance, string? ___lightSourceId, GameTime time, GameLocation environment, Farmer who)
    {
        try
        {
            if (___lightSourceId == null)
            {
                return false;
            }
            Vector2 zero = Vector2.Zero;
            if (who.shouldShadowBeOffset)
            {
                zero += who.drawOffset;
            }

            environment.repositionLightSource(___lightSourceId, new Vector2(who.Position.X + 21f, who.Position.Y) + zero);

            return false;

        }

        catch (Exception ex)
        {
            Monitor?.Log($"Failed in patched code: {nameof(Update_Prefix)}:\n{ex}", LogLevel.Error);
            return true;
        }
    }
}
