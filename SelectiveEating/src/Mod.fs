namespace SelectiveEating

open StardewValley
open StardewModdingAPI
open CloudyCore.Prelude
open CloudyCore.IGenericConfigMenuApi
open SelectiveEating.Config
open VitalsSelector

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

    member private this.OnGameLaunched (event : Events.GameLaunchedEventArgs) =
        let hyratedConfigMenuAPI =
            this.Helper.ModRegistry.GetApi<IGenericConfigMenuApi>
                IGenericConfigMenuApi.Constant.INTERFACE_ID

        GenericConfigMennu(
            this.Config,
            this.Helper,
            this.ModManifest,
            hyratedConfigMenuAPI
        )
            .Setup ()

    member private this.OnButtonPressed
        (event : Events.ButtonPressedEventArgs)
        =
        this.Config
        |> Option.iter (fun config ->
            if
                Context.IsPlayerFree
                && event.Button = config.SelectiveEatingShortcutButtonInput
            then
                this.Config.Value.IsSelectiveEatingModEnabled <-
                    not this.Config.Value.IsSelectiveEatingModEnabled

                this.ShowKeybindModToggleHUDMessage
                    this.Config.Value.IsSelectiveEatingModEnabled
        )

    member private this.ShowKeybindModToggleHUDMessage (isEnabled : bool) =
        let modStatus = if isEnabled then "enabled" else "disabled"

        let message =
            this.Helper.Translation.Get (
                "keybind-toggle-hud-message",
                {| modIsEnabled = modStatus |}
            )

        let hudMessage = HUDMessage message
        hudMessage.noIcon <- true
        Game1.addHUDMessage hudMessage

    member private this.OnUpdateTicked (event : Events.UpdateTickedEventArgs) =
        this.Config
        |> Option.iter (fun config ->
            if Context.IsWorldReady && config.IsSelectiveEatingModEnabled then
                let player = Game1.player

                let threshold = config.ThresholdCheckPerSecond

                let isTimeToProcess =
                    event.IsMultipleOf (
                        if threshold > 0u then threshold * 60u else 20u
                    )

                let arePlayerVitalsLow =
                    makePlayerHealth player
                    <= config.MinimumHealthToStartAutoEat
                    || makePlayerStamina player
                       <= config.MinimumStaminaToStartAutoEat

                if isTimeToProcess && arePlayerVitalsLow then
                    this.TryEatFood (config, player)
        )

    member private this.TryEatFood (config : ModConfig, player : Farmer) =
        InventoryFood.tryGetFoodItem config player
        |> Option.iter (fun item ->
            let lastPlayerDirection = player.FacingDirection

            this.EatFood (lastPlayerDirection, config, player, item.Food)
        )

    member private this.EatFood
        (
            lastPlayerDirectionBeforeEating : int,
            config : ModConfig,
            player : Farmer,
            food : Food
        )
        =
        let shouldEatWithAnimation =
            not config.IsSkipEatingAnimationEnabled && Context.CanPlayerMove

        if config.IsSkipEatingAnimationEnabled then
            this.EatWithoutAnimation (player, food)
        elif shouldEatWithAnimation then
            this.EatWithAnimation (
                lastPlayerDirectionBeforeEating,
                config,
                player,
                food
            )

    member private this.EatWithAnimation
        (
            lastPlayerDirectionBeforeEating : int,
            config : ModConfig,
            player : Farmer,
            food : Food
        )
        =
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
