namespace CrabPotCollectJellies;

using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Objects;

internal class ModEntry : Mod
{

    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);
        CrabPotPatcher.Initialize(this.Monitor);

        harmony.Patch(
            original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
            prefix: new HarmonyMethod(typeof(CrabPotPatcher), nameof(CrabPotPatcher.DayUpdate_Prefix))
        );

    }
}
