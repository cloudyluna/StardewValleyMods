namespace SelectiveEating

open StardewModdingAPI
open CloudyCore.IGenericConfigMenuApi

// For translations
#nowarn "3391"

module Config =
  open System

  [<AllowNullLiteral>]
  type ModConfig() =
    member val IsSelectiveEatingModEnabled = true with get, set
    member val SelectiveEatingShortcutButtonInput = SButton.L with get, set
    member val MinimumHealthToStartAutoEat = 80 with get, set
    member val MinimumStaminaToStartAutoEat = 80 with get, set
    member val PriorityStrategySelection = "health_or_stamina" with get, set
    member val IsSkipEatingAnimationEnabled = false with get, set
    member val ThresholdCheckPerSecond = 1u with get, set
    member val ForbiddenFoods = "" with get, set
    member val IsStayInLastDirectionEnabled = true with get, set


  let parsedForbiddenFood (str : string) =
    str.Trim().Split (',', StringSplitOptions.RemoveEmptyEntries)
    |> Array.map (fun i -> i.Trim ())

  type FoodPriorityStrategy =
    | HealthOrStamina
    | CheapestFood
    | Off
    | Invalid of string

  let priorityStrategyFromString s =
    match s with
    | "HealthOrStamina" -> HealthOrStamina
    | "CheapestFood" -> CheapestFood
    | "Off" -> Off
    | _other -> Invalid _other

  type GenericConfigMennu
    (
      Config : ModConfig option,
      Helper : IModHelper,
      Manifest : IManifest,
      Api : IGenericConfigMenuApi
    )
    =

    let mutable _config = Config
    let mutable _helper = Helper
    let mutable _manifest = Manifest
    let mutable _api = Api

    member private _.config
      with get () = _config
      and set x = _config <- x

    member private _.helper
      with get () = _helper
      and set x = _helper <- x

    member private _.manifest
      with get () = _manifest
      and set x = _manifest <- x

    member private _.api
      with get () = _api
      and set x = _api <- x

    member this.Setup () =
      this.api
      |> Option.ofObj
      |> Option.iter (fun menu ->
        menu.Register (
          manifest = this.manifest,
          reset = (fun _ -> this.config <- Some (ModConfig ())),
          save = (fun _ -> this.helper.WriteConfig this.config.Value),
          titleScreenOnly = false
        )

        this.config
        |> Option.iter (fun config ->

          let i18n = this.helper.Translation.Get

          menu.AddSectionTitle (
            manifest = this.manifest,
            text = (fun _ -> i18n "menu.basics.title"),
            tooltip = null
          )

          menu.AddBoolOption (
            manifest = this.manifest,
            getValue = (fun _ -> config.IsSelectiveEatingModEnabled),
            setValue =
              (fun b -> this.config.Value.IsSelectiveEatingModEnabled <- b),
            name = (fun _ -> i18n "menu.basics.enable-mod"),
            tooltip = null,
            fieldId = null
          )

          menu.AddNumberOption (
            manifest = this.manifest,
            getValue = (fun _ -> int config.MinimumHealthToStartAutoEat),
            setValue =
              (fun v -> this.config.Value.MinimumHealthToStartAutoEat <- v),
            name = (fun _ -> i18n "menu.basics.minimum-health"),
            tooltip = (fun _ -> i18n "menu.basics.minimum-health.tooltip"),
            min = 0,
            max = 80,
            interval = 10,
            formatValue = null,
            fieldId = null
          )

          menu.AddNumberOption (
            manifest = this.manifest,
            getValue = (fun _ -> int config.MinimumStaminaToStartAutoEat),
            setValue =
              (fun v ->
                this.config.Value.MinimumStaminaToStartAutoEat <- int v
              ),
            name = (fun _ -> i18n "menu.basics.minimum-stamina"),
            tooltip = (fun _ -> i18n "menu.basics.minimum-stamina.tooltip"),
            min = 0,
            max = 80,
            interval = 10,
            formatValue = null,
            fieldId = null
          )

          menu.AddTextOption (
            manifest = this.manifest,
            getValue = (fun _ -> config.PriorityStrategySelection),
            setValue =
              (fun v -> this.config.Value.PriorityStrategySelection <- v),
            name = (fun _ -> i18n "menu.basics.priority-strategy"),
            tooltip = (fun _ -> i18n "menu.basics.priority-strategy.tooltip"),
            allowedValues =
              [|
                FoodPriorityStrategy.HealthOrStamina.ToString ()
                FoodPriorityStrategy.CheapestFood.ToString ()
                FoodPriorityStrategy.Off.ToString ()
              |],
            formatAllowedValue =
              (fun unformatted ->
                match priorityStrategyFromString unformatted with
                | HealthOrStamina -> "Health or Stamina"
                | CheapestFood -> "Cheapest Food"
                | Off -> "Off"
                | Invalid _other ->
                  failwith
                    $"Invalid key: {_other}. Key can only be either: HealthOrStamina, CheapestFood or Off"
              ),
            fieldId = null
          )

          menu.AddSectionTitle (
            manifest = this.manifest,
            text = (fun _ -> i18n "menu.basics.priority.subsection.title"),
            tooltip = null
          )

          menu.AddParagraph (
            manifest = this.manifest,
            text = (fun _ -> i18n "menu.basics.priority.subsection")
          )

          menu.AddBoolOption (
            manifest = this.manifest,
            getValue = (fun _ -> config.IsSkipEatingAnimationEnabled),
            setValue =
              (fun b -> this.config.Value.IsSkipEatingAnimationEnabled <- b),
            name = (fun _ -> i18n "menu.basics.enable-skip-eating-animation"),
            tooltip =
              (fun _ ->
                i18n "menu.basics.enable-skip-eating-animation.tooltip"
              ),
            fieldId = null
          )

          menu.AddSectionTitle (
            manifest = this.manifest,
            text = (fun _ -> i18n "menu.advanced.title"),
            tooltip = (fun _ -> i18n "menu.advanced.title.tooltip")
          )

          menu.AddKeybind (
            manifest = this.manifest,
            getValue = (fun _ -> config.SelectiveEatingShortcutButtonInput),
            setValue =
              (fun b ->
                this.config.Value.SelectiveEatingShortcutButtonInput <- b
              ),
            name = (fun () -> i18n "menu.advanced.enable-mod-keybind"),
            tooltip =
              (fun () -> i18n "menu.advanced.enable-mod-keybind.tooltip"),
            fieldId = null
          )

          menu.AddNumberOption (
            manifest = this.manifest,
            getValue = (fun _ -> int config.ThresholdCheckPerSecond),
            setValue =
              (fun v -> this.config.Value.ThresholdCheckPerSecond <- uint v),
            name = (fun _ -> i18n "menu.advanced.check-seconds-threshold"),
            tooltip =
              (fun _ -> i18n "menu.advanced.check-seconds-threshold.tooltip"),
            min = 0,
            max = 60,
            interval = 1,
            formatValue = null,
            fieldId = null
          )

          menu.AddTextOption (
            manifest = this.manifest,
            getValue = (fun _ -> config.ForbiddenFoods),
            setValue = (fun v -> this.config.Value.ForbiddenFoods <- v),
            name = (fun _ -> i18n "menu.advanced.forbidden-food"),
            tooltip = (fun _ -> i18n "menu.advanced.forbidden-food.tooltip"),
            allowedValues = null,
            formatAllowedValue = null,
            fieldId = null
          )

          menu.AddParagraph (
            manifest = this.manifest,
            text =
              (fun _ -> i18n "menu.advanced.forbidden-food.usage-example")
          )

          menu.AddBoolOption (
            manifest = this.manifest,
            getValue = (fun _ -> config.IsStayInLastDirectionEnabled),
            setValue =
              (fun b -> this.config.Value.IsStayInLastDirectionEnabled <- b),
            name = (fun _ -> i18n "menu.advanced.stay-in-last-direction"),
            tooltip =
              (fun _ -> i18n "menu.advanced.stay-in-last-direction.tooltip"),
            fieldId = null
          )
        )
      )
