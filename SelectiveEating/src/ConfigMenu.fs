namespace SelectiveEating

open StardewModdingAPI
open CloudyCore.IGenericConfigMenuApi
open CloudyCore.Prelude

type FoodPriorityStrategy =
    | HealthOrStamina
    | CheapestFood
    | Off
    | Invalid of string

module private Aux =
    let priorityStrategyFromString s =
        match s with
        | "HealthOrStamina" -> HealthOrStamina
        | "CheapestFood" -> CheapestFood
        | "Off" -> Off
        | _other -> Invalid _other

type ConfigMenu
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
        Option.iter this.TryMakeGenericConfigMenu this.config

    member private this.TryMakeGenericConfigMenu (config : ModConfig) =
        let mutable config = config

        let tryRegister =
            this.api
            |> Option.ofObj
            |> Option.bind (fun menu ->
                menu.Register (
                    manifest = this.manifest,
                    reset = (fun _ -> config <- ModConfig ()),
                    save = (fun _ -> this.helper.WriteConfig config),
                    titleScreenOnly = false
                )

                Some menu
            )

        Option.exec tryRegister
        ^ fun menu ->
            let translate = Translation.stringKey this.helper.Translation


            menu.AddSectionTitle (
                manifest = this.manifest,
                text = translate "menu.basics.title",
                tooltip = null
            )

            menu.AddBoolOption (
                manifest = this.manifest,
                getValue = (fun _ -> config.IsSelectiveEatingModEnabled),
                setValue = (fun b -> config.IsSelectiveEatingModEnabled <- b),
                name = translate "menu.basics.enable-mod",
                tooltip = null,
                fieldId = null
            )

            menu.AddNumberOption (
                manifest = this.manifest,
                getValue = (fun _ -> config.MinimumHealthToStartAutoEat),
                setValue = (fun v -> config.MinimumHealthToStartAutoEat <- v),
                name = translate "menu.basics.minimum-health",
                tooltip = translate "menu.basics.minimum-health.tooltip",
                min = 0,
                max = 80,
                interval = 10,
                formatValue = null,
                fieldId = null
            )

            menu.AddNumberOption (
                manifest = this.manifest,
                getValue = (fun _ -> config.MinimumStaminaToStartAutoEat),
                setValue = (fun v -> config.MinimumStaminaToStartAutoEat <- v),
                name = translate "menu.basics.minimum-stamina",
                tooltip = translate "menu.basics.minimum-stamina.tooltip",
                min = 0,
                max = 80,
                interval = 10,
                formatValue = null,
                fieldId = null
            )

            menu.AddTextOption (
                manifest = this.manifest,
                getValue = (fun _ -> config.PriorityStrategySelection),
                setValue = (fun v -> config.PriorityStrategySelection <- v),
                name = translate "menu.basics.priority-strategy",
                tooltip = translate "menu.basics.priority-strategy.tooltip",
                allowedValues =
                    [|
                        FoodPriorityStrategy.HealthOrStamina.ToString ()
                        FoodPriorityStrategy.CheapestFood.ToString ()
                        FoodPriorityStrategy.Off.ToString ()
                    |],
                formatAllowedValue =
                    (fun unformatted ->
                        match Aux.priorityStrategyFromString unformatted with
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
                text = translate "menu.basics.priority.subsection.title",
                tooltip = null
            )

            menu.AddParagraph (
                manifest = this.manifest,
                text = translate "menu.basics.priority.subsection"
            )

            menu.AddBoolOption (
                manifest = this.manifest,
                getValue = (fun _ -> config.IsSkipEatingAnimationEnabled),
                setValue = (fun b -> config.IsSkipEatingAnimationEnabled <- b),
                name = translate "menu.basics.enable-skip-eating-animation",
                tooltip =
                    translate "menu.basics.enable-skip-eating-animation.tooltip",
                fieldId = null
            )

            menu.AddSectionTitle (
                manifest = this.manifest,
                text = translate "menu.advanced.title",
                tooltip = translate "menu.advanced.title.tooltip"
            )

            menu.AddKeybind (
                manifest = this.manifest,
                getValue = (fun _ -> config.SelectiveEatingShortcutButtonInput),
                setValue =
                    (fun b -> config.SelectiveEatingShortcutButtonInput <- b),
                name = translate "menu.advanced.enable-mod-keybind",
                tooltip = translate "menu.advanced.enable-mod-keybind.tooltip",
                fieldId = null
            )

            menu.AddNumberOption (
                manifest = this.manifest,
                getValue = (fun _ -> int config.ThresholdCheckPerSecond),
                setValue = (fun v -> config.ThresholdCheckPerSecond <- uint v),
                name = translate "menu.advanced.check-seconds-threshold",
                tooltip =
                    translate "menu.advanced.check-seconds-threshold.tooltip",
                min = 0,
                max = 60,
                interval = 1,
                formatValue = null,
                fieldId = null
            )

            menu.AddTextOption (
                manifest = this.manifest,
                getValue = (fun _ -> config.ForbiddenFoods),
                setValue = (fun v -> config.ForbiddenFoods <- v),
                name = translate "menu.advanced.forbidden-food",
                tooltip = translate "menu.advanced.forbidden-food.tooltip",
                allowedValues = null,
                formatAllowedValue = null,
                fieldId = null
            )

            menu.AddParagraph (
                manifest = this.manifest,
                text = translate "menu.advanced.forbidden-food.usage-example"
            )

            menu.AddBoolOption (
                manifest = this.manifest,
                getValue = (fun _ -> config.IsStayInLastDirectionEnabled),
                setValue = (fun b -> config.IsStayInLastDirectionEnabled <- b),
                name = translate "menu.advanced.stay-in-last-direction",
                tooltip =
                    translate "menu.advanced.stay-in-last-direction.tooltip",
                fieldId = null
            )
