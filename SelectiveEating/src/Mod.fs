namespace SelectiveEating

open StardewValley
open StardewModdingAPI
open CloudyCore.Prelude
open CloudyCore.IGenericConfigMenuApi
open CloudyCore.Prelude.Math
open SelectiveEating.Config

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

  member private this.OnButtonPressed (event : Events.ButtonPressedEventArgs) =
    this.Config
    |> Option.iter (fun config ->
      if
        Context.IsPlayerFree
        && event.Button = config.SelectiveEatingShortcutButtonInput
      then
        this.Config.Value.IsSelectiveEatingModEnabled <-
          not this.Config.Value.IsSelectiveEatingModEnabled
    )

  member private this.OnUpdateTicked (event : Events.UpdateTickedEventArgs) =
    this.Config
    |> Option.iter (fun config ->
      if Context.IsWorldReady && config.IsSelectiveEatingModEnabled then
        let player = Game1.player

        let isTimeToProcess =
          event.IsMultipleOf (config.ThresholdCheckPerSecond * 60u)

        let health = ofPercentage player.health player.maxHealth

        let stamina = ofPercentage (int player.stamina) player.MaxStamina

        let arePlayerVitalsLow =
          health <= config.MinimumHealthToStartAutoEat
          || stamina <= config.MinimumStaminaToStartAutoEat

        if isTimeToProcess && arePlayerVitalsLow then
          this.TryEatFood (Context.CanPlayerMove, config, player)
    )

  member private this.TryEatFood
    (canPlayerMove : bool, config : ModConfig, player : Farmer)
    =
    InventoryFood.tryGetFoodItem config player
    |> Option.iter (fun item ->
      let lastPlayerDirection = player.FacingDirection

      this.EatFood (
        canPlayerMove,
        lastPlayerDirection,
        config,
        player,
        item.Food
      )
    )

  member private this.EatFood
    (
      canPlayerMove : bool,
      lastPlayerDirectionBeforeEating : int,
      config : ModConfig,
      player : Farmer,
      food : Food
    )
    =
    let decreaseFood () =

      food.Stack <- food.Stack - 1

      if food.Stack = 0 then
        player.removeItemFromInventory food

    let canEatWithAnimation =
      not config.IsSkipEatingAnimationEnabled && canPlayerMove

    if config.IsSkipEatingAnimationEnabled then
      this.EatWithoutAnimation (player, food)
      decreaseFood ()
    elif canEatWithAnimation then
      player.eatObject (food, false)

      if config.IsStayInLastDirectionEnabled then
        player.FacingDirection <- lastPlayerDirectionBeforeEating

      decreaseFood ()

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
