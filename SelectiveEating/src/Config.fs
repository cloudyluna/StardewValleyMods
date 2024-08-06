namespace SelectiveEating

open StardewModdingAPI
open CloudyCore.IGenericConfigMenuApi

// For translations
#nowarn "3391"

module Config =

  [<AllowNullLiteral>]
  type ModConfig() =
    member val IsSelectiveEatingModEnabled = true with get, set
    member val SelectiveEatingShortcutButtonInput = SButton.L with get, set
    member val MinimumHealthToStartAutoEat = 60 with get, set
    member val MinimumStaminaToStartAutoEat = 20 with get, set
    member val ThresholdCheckPerSecond = 2u with get, set
    member val ForbiddenFoods = "" with get, set
    member val PrioritizedFoods = "" with get, set
    member val StayInLastDirectionToggle = true with get, set


  type private Slider =
    {
      Min : int
      Max : int
      Interval : int
    }

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
          this.manifest,
          (fun _ -> this.config <- Some (ModConfig ())),
          (fun _ -> this.helper.WriteConfig this.config.Value),
          false
        )

        this.config
        |> Option.iter (fun config ->

          let i18n = this.helper.Translation.Get

          menu.AddSectionTitle (
            this.manifest,
            (fun _ -> i18n "menu.basics.title"),
            null
          )

          menu.AddBoolOption (
            this.manifest,
            (fun _ -> config.IsSelectiveEatingModEnabled),
            (fun b -> this.config.Value.IsSelectiveEatingModEnabled <- b),
            (fun _ -> i18n "menu.basics.enable-mod"),
            null,
            null
          )

          let healthOpt : Slider = { Min = 0 ; Max = 80 ; Interval = 10 }

          menu.AddNumberOption (
            this.manifest,
            (fun _ -> int config.MinimumHealthToStartAutoEat),
            (fun v -> this.config.Value.MinimumHealthToStartAutoEat <- v),
            (fun _ -> i18n "menu.basics.minimum-health"),
            (fun _ -> i18n "menu.basics.minimum-health.tooltip"),
            healthOpt.Min,
            healthOpt.Max,
            healthOpt.Interval,
            null,
            null
          )

          let staminaOpt : Slider = { Min = 0 ; Max = 80 ; Interval = 10 }

          menu.AddNumberOption (
            this.manifest,
            (fun _ -> int config.MinimumStaminaToStartAutoEat),
            (fun v -> this.config.Value.MinimumStaminaToStartAutoEat <- int v),
            (fun _ -> i18n "menu.basics.minimum-stamina"),
            (fun _ -> i18n "menu.basics.minimum-stamina.tooltip"),
            staminaOpt.Min,
            staminaOpt.Max,
            staminaOpt.Interval,
            null,
            null
          )

          menu.AddSectionTitle (
            this.manifest,
            (fun _ -> i18n "menu.advanced.title"),
            (fun _ -> i18n "menu.advanced.title.tooltip")
          )

          menu.AddKeybind (
            this.manifest,
            (fun _ -> config.SelectiveEatingShortcutButtonInput),
            (fun b ->
              this.config.Value.SelectiveEatingShortcutButtonInput <- b
            ),
            (fun () -> i18n "menu.advanced.enable-mod-keybind"),
            (fun () -> i18n "menu.advanced.enable-mod-keybind.tooltip"),
            ""
          )

          let checkSecondsOpt : Slider = { Min = 1 ; Max = 60 ; Interval = 1 }

          menu.AddNumberOption (
            this.manifest,
            (fun _ -> int config.ThresholdCheckPerSecond),
            (fun v -> this.config.Value.ThresholdCheckPerSecond <- uint v),
            (fun _ -> i18n "menu.advanced.check-seconds-threshold"),
            (fun _ -> i18n "menu.advanced.check-seconds-threshold.tooltip"),
            checkSecondsOpt.Min,
            checkSecondsOpt.Max,
            checkSecondsOpt.Interval,
            null,
            null
          )

          menu.AddTextOption (
            this.manifest,
            (fun _ -> config.ForbiddenFoods),
            (fun v -> this.config.Value.ForbiddenFoods <- v),
            (fun _ -> i18n "menu.advanced.forbidden-food"),
            (fun _ -> i18n "menu.advanced.forbidden-food.tooltip"),
            null,
            null,
            null
          )

          menu.AddParagraph (
            this.manifest,
            (fun _ -> i18n "menu.advanced.forbidden-food.usage-example")
          )

          menu.AddParagraph (
            this.manifest,
            (fun _ -> i18n "menu.advanced.forbidden-food.subsection")
          )

          menu.AddTextOption (
            this.manifest,
            (fun _ -> config.PrioritizedFoods),
            (fun v -> this.config.Value.PrioritizedFoods <- v),
            (fun _ -> i18n "menu.advanced.prioritized-food"),
            (fun _ -> i18n "menu.advanced.prioritized-food.tooltip"),
            null,
            null,
            null
          )

          menu.AddParagraph (
            this.manifest,
            (fun _ -> i18n "menu.advanced.prioritized-food.usage-example")
          )

          menu.AddBoolOption (
            this.manifest,
            (fun _ -> config.StayInLastDirectionToggle),
            (fun b -> this.config.Value.StayInLastDirectionToggle <- b),
            (fun _ -> i18n "menu.advanced.stay-in-last-direction"),
            (fun _ -> i18n "menu.advanced.stay-in-last-direction.tooltip"),
            null
          )
        )
      )
