namespace SelectiveEating

open StardewValley
open StardewModdingAPI
open CloudyCore.Prelude
open CloudyCore.IGenericConfigMenuApi

type internal Mod() =
    inherit StardewModdingAPI.Mod()

    member private this.debugMsg = debug this.Monitor
    member val Config : ModConfig option = None with get, set

    override this.Entry (helper : IModHelper) : unit =

        this.Config <- Option.ofObj <| this.Helper.ReadConfig<ModConfig> ()

        let events = helper.Events

        events.GameLoop.GameLaunched.Add this.OnGameLaunched
        events.Input.ButtonPressed.Add this.OnButtonPressed
        events.GameLoop.UpdateTicked.Add this.OnUpdateTicked

    member private this.OnGameLaunched (_ : Events.GameLaunchedEventArgs) =
        let hyratedConfigMenuAPI =
            this.Helper.ModRegistry.GetApi<IGenericConfigMenuApi>
                IGenericConfigMenuApi.Constant.INTERFACE_ID

        ConfigMenu(
            this.Config,
            this.Helper,
            this.ModManifest,
            hyratedConfigMenuAPI
        )
            .Setup ()

    member private this.OnButtonPressed
        (event : Events.ButtonPressedEventArgs)
        =

        Option.exec this.Config
        ^ fun config ->
            if
                Context.IsPlayerFree
                && event.Button = config.SelectiveEatingShortcutButtonInput
            then
                config.IsSelectiveEatingModEnabled <-
                    not config.IsSelectiveEatingModEnabled

                this.Helper.WriteConfig config

                this.ShowKeybindModToggleHUDMessage
                    config.IsSelectiveEatingModEnabled

    member private this.ShowKeybindModToggleHUDMessage (isEnabled : bool) =
        let modStatus = if isEnabled then "enabled" else "disabled"

        // TODO: Translate enabled/disabled too!

        let message =
            Translation.genericKey
                this.Helper.Translation
                "keybind-toggle-hud-message"
                {| modIsEnabled = modStatus |}

        let hudMessage = HUDMessage message
        hudMessage.noIcon <- true
        Game1.addHUDMessage hudMessage


    member private this.OnUpdateTicked (e : Events.UpdateTickedEventArgs) =
        Option.exec this.Config
        ^ fun config ->
            if Context.IsWorldReady && config.IsSelectiveEatingModEnabled then
                let isTimeToProcess =
                    let threshold = config.ThresholdCheckPerSecond
                    let second, third = 60u, 20u

                    e.IsMultipleOf (
                        if threshold > 0u then threshold * second else third
                    )

                if isTimeToProcess then
                    let player = Game1.player

                    let priority =
                        VitalsSelector.getVitalsPriority config player

                    this.TryEatFood (config, player, priority)

    member private this.TryEatFood
        (config : ModConfig, player : Farmer, vitalsPriority : VitalsPriority)
        =
        Option.exec (InventoryFood.tryGetOne config player vitalsPriority)
        ^ fun item -> this.EatFood (config, player, item.Food)

    member private this.EatFood
        (config : ModConfig, player : Farmer, food : Food)
        =
        let shouldEatWithAnimation =
            not config.IsSkipEatingAnimationEnabled && Context.CanPlayerMove

        if config.IsSkipEatingAnimationEnabled then
            this.EatWithoutAnimation (player, food)
        elif shouldEatWithAnimation then
            this.EatWithAnimation (config, player, food)

    member private this.EatWithAnimation
        (config : ModConfig, player : Farmer, food : Food)
        =
        let lastPlayerDirectionBeforeEating = player.FacingDirection

        player.eatObject (food, false)

        if config.IsStayInLastDirectionEnabled then
            player.FacingDirection <- lastPlayerDirectionBeforeEating

        this.DecreaseFood (player, food)

    member private this.EatWithoutAnimation (player : Farmer, food : Food) =
        player.health <-
            min
                player.maxHealth
                (player.health + food.healthRecoveredOnConsumption ())

        player.stamina <-
            float32
            <| min
                player.MaxStamina
                (int player.stamina + food.staminaRecoveredOnConsumption ())

        this.DecreaseFood (player, food)

    member private this.DecreaseFood (player : Farmer, food : Food) =
        food.Stack <- food.Stack - 1

        if food.Stack = 0 then
            player.removeItemFromInventory food
