namespace DropSeedsAfterEating;

using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using HarmonyLib;
using System.Net.NetworkInformation;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);

        FarmerPatcher.Initialize(this.Monitor);

        harmony.Patch(
            original: AccessTools.Method(typeof(Farmer), nameof(Farmer.eatObject)),
            postfix: new HarmonyMethod(typeof(FarmerPatcher), nameof(FarmerPatcher.eatObject_Postfix))
        );

    }
}
