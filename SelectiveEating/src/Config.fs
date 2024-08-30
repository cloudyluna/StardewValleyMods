namespace SelectiveEating

open StardewModdingAPI

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
