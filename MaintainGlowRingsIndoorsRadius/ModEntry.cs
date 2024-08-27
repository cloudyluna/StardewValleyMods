namespace MaintainGlowRingsIndoorsRadius;

using StardewModdingAPI;
using HarmonyLib;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);

        RingPatcher.Initialize(this.Monitor);

        harmony.Patch(
            original: AccessTools.Method(typeof(StardewValley.Objects.Ring), nameof(StardewValley.Objects.Ring.update)),
            prefix: new HarmonyMethod(typeof(RingPatcher), nameof(RingPatcher.Update_Prefix))
        );
    }
}
