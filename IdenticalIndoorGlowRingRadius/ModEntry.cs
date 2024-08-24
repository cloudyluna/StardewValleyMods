namespace IdenticalIndoorGlowRingRadius;

using StardewModdingAPI;
using HarmonyLib;

internal class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var harmony = new Harmony(this.ModManifest.UniqueID);

        harmony.Patch(
            original: AccessTools.Method(typeof(StardewValley.Objects.Ring), nameof(StardewValley.Objects.Ring.onEquip)),
            prefix: new HarmonyMethod(typeof(RingPatcher), nameof(RingPatcher.OnEquip_Prefix))
        );

    }
}
