namespace SelectiveEating

open StardewValley
open StardewModdingAPI
open CloudyCore.Prelude
open CloudyCore.IGenericConfigMenuApi
open VitalsSelector
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


        if Context.CanPlayerMove && isTimeToProcess && arePlayerVitalsLow then
          let lastPlayerDirection = player.FacingDirection

          InventoryFood.tryGetFoodItem config player
          |> Option.iter (fun item ->
            this.EatFood (player, item.Food)
            player.FacingDirection <- lastPlayerDirection
          )
    )

  member private this.EatFood (player : Farmer, food : Food) =

    player.eatObject (food, false)
    food.Stack <- food.Stack - 1

    if food.Stack = 0 then
      player.removeItemFromInventory food
