namespace SelectiveEating.Config

open StardewModdingAPI

type FoodPriorityStrategy =
    | HealthOrStamina
    | CheapestFood
    | Off
    | Invalid of string

[<AllowNullLiteral>]
type ModConfig() =
    member val IsSelectiveEatingModEnabled = true with get, set
    member val SelectiveEatingShortcutButtonInput = SButton.L with get, set
    member val MinimumHealthToStartAutoEat = 80 with get, set
    member val MinimumStaminaToStartAutoEat = 80 with get, set

    member val PriorityStrategySelection =
        HealthOrStamina.ToString () with get, set

    member val IsSkipEatingAnimationEnabled = false with get, set
    member val ThresholdCheckPerSecond = 0u with get, set
    member val ForbiddenFoods = "" with get, set
    member val IsStayInLastDirectionEnabled = true with get, set
