namespace SelectiveEating

open StardewValley
open StardewModdingAPI
open CloudyCore.Prelude
open CloudyCore.IGenericConfigMenuApi
open SelectiveEating.API

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

                    // player.eatObject() doesn't block! We gotta
                    // check if player is trully "free" first to eat
                    // again before actually attempt to proceed
                    // with this.TryEatFood.
                    // Not doing this will cause the player to
                    // this.TryEatFood more than once
                    // and can get real expensive at the lowest
                    // condition checking interval.
                    //
                    // This can be ignored for skipped eating animation
                    // because all it does is just updating the actual
                    // stats behind the scene.
                    let isDoingEatingAnimation =
                        not config.IsSkipEatingAnimationEnabled
                        && not Context.CanPlayerMove
                        || player.isEating

                    if isDoingEatingAnimation then
                        ()
                    else
                        this.TryEatFood (config, player)

    member private this.TryEatFood (config : ModConfig, player : Farmer) =
        Option.exec (InventoryFood.tryGetOne config player)
        ^ fun food -> this.EatFood (config, player, food)

    member private this.EatFood
        (config : ModConfig, player : Farmer, food : Food)
        =
        let shouldEatWithAnimation =
            not config.IsSkipEatingAnimationEnabled && Context.CanPlayerMove

        let foodObj = player.Items.GetById food.Id |> Seq.head :?> FoodObject

        if config.IsSkipEatingAnimationEnabled then
            this.EatWithoutAnimation (player, foodObj)
        elif shouldEatWithAnimation then
            this.EatWithAnimation (config, player, foodObj)

    member private this.EatWithAnimation
        (config : ModConfig, player : Farmer, food : FoodObject)
        =
        let lastPlayerDirectionBeforeEating = player.FacingDirection

        player.eatObject (food, false)

        if config.IsStayInLastDirectionEnabled then
            player.FacingDirection <- lastPlayerDirectionBeforeEating

        this.DecreaseFood (player, food)

    member private this.EatWithoutAnimation
        (player : Farmer, food : FoodObject)
        =

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

    member private this.DecreaseFood (player : Farmer, food : FoodObject) =
        food.Stack <- food.Stack - 1

        if food.Stack = 0 then
            player.removeItemFromInventory food
