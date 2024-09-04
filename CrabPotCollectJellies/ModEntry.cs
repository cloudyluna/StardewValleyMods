namespace CrabPotCollectJellies;

using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewModdingAPI.Events;

internal class ModEntry : Mod
{

    private ModConfig? Config;

    public override void Entry(IModHelper helper)
    {
        this.Config = this.Helper.ReadConfig<ModConfig>();

        var harmony = new Harmony(this.ModManifest.UniqueID);

        CrabPotPatcher.Initialize(this.Monitor, this.Config);

        harmony.Patch(
            original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
            prefix: new HarmonyMethod(typeof(CrabPotPatcher), nameof(CrabPotPatcher.DayUpdate_Prefix))
        );

        var events = this.Helper.Events;
        events.GameLoop.GameLaunched += OnGameLaunched;


    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu =
            this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null || this.Config is null)
            return;


        // register mod
        configMenu.Register(
            mod: this.ModManifest,
            reset: () => this.Config = new ModConfig(),
            save: () => this.Helper.WriteConfig(this.Config)
        );

        var i18n = this.Helper.Translation;
        configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => i18n.Get("menu.basics.enable-mod"),
            tooltip: () => i18n.Get("menu.basics.enable-mod.tooltip"),
            getValue: () => this.Config.IsModEnabled,
            setValue: value => this.Config.IsModEnabled = value
        );


        configMenu.AddBoolOption(
            mod: this.ModManifest,
            name: () => i18n.Get("menu.basics.enable-trash-replacer"),
            tooltip: () => i18n.Get("menu.basics.enable-trash-replacer.tooltip"),
            getValue: () => this.Config.IsReplaceAllTrashSelected,
            setValue: value => this.Config.IsReplaceAllTrashSelected = value
        );
    }
}
