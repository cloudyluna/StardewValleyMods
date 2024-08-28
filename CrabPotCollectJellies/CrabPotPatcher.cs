namespace CrabPotCollectJellies;

using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;
using StardewValley.Extensions;

internal class CrabPotPatcher
{
    private static IMonitor? Monitor;

    internal static void Initialize(IMonitor monitor)
    {
        Monitor = monitor;
    }

    internal static void DayUpdate_Postfix(CrabPot __instance)
    {
        try
        {
            var crabPot = __instance;

            if (crabPot.heldObject.Value == null)
            {
                Random random = Utility.CreateDaySaveRandom(
                    crabPot.TileLocation.X * 1000f,
                    crabPot.TileLocation.Y * 255f,
                    crabPot.directionOffset.X * 1000f +
                    crabPot.directionOffset.Y
                );

                var nonFishIds = new List<string> { "SeaJelly", "RiverJelly", "CaveJelly", random.Next(168, 173).ToString() };
                var chosenId = random.ChooseFrom(nonFishIds);
                crabPot.heldObject.Value = ItemRegistry.Create<Object>("(O)" + chosenId);
            }
        }

        catch (Exception ex)
        {
            Monitor?.Log($"");
            Monitor?.Log(
                $"Failed in patched code of: {nameof(DayUpdate_Postfix)}:\n{ex}",
                LogLevel.Error
            );
        }
    }
}