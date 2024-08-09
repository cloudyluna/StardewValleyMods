namespace SelectiveEating

open StardewValley
open SelectiveEating.Config
open CloudyCore.Prelude.Math

type Food = Object
type FoodItem = { Food : Food ; IsPriority : bool }

type VitalStatus = { Current : Percentage ; Max : int }

type VitalsPriority =
  | Health of VitalStatus
  | Stamina of VitalStatus
  | DoingOK

module VitalsSelector =

  /// Health will always be the biggest priority for vitals.
  let getVitalsPriority
    (config : ModConfig)
    (player : Farmer)
    : VitalsPriority
    =

    let health = ofPercentage player.health player.maxHealth
    let stamina = ofPercentage (int player.stamina) player.MaxStamina

    let makeVitalStatus current max =
      {
        Current = makePercentage current max
        Max = max
      }

    if health <= config.MinimumHealthToStartAutoEat then
      Health <| makeVitalStatus player.health player.maxHealth
    elif stamina <= config.MinimumStaminaToStartAutoEat then
      Stamina <| makeVitalStatus (int player.stamina) player.maxHealth
    else
      DoingOK


  let private closestNeededToReplenish recoveredAmount amountToRefill maxStat =
    abs (ofPercentage (recoveredAmount ()) maxStat - amountToRefill)

  let getMostFittingHealthRevitalizingFood
    (vitalStat : VitalStatus)
    (edibles : FoodItem array)
    : FoodItem
    =

    let health = fromPercentage vitalStat.Current
    let maxHealth = vitalStat.Max
    let amountToRefill = maxHealth - health

    edibles
    |> Array.minBy (fun item ->
      closestNeededToReplenish
        item.Food.healthRecoveredOnConsumption
        amountToRefill
        maxHealth
    )

  let getMostFittingStaminaRevitalizingFood
    (vital : VitalStatus)
    (edibles : FoodItem array)
    : FoodItem
    =
    let stamina = fromPercentage vital.Current
    let maxStamina = vital.Max
    let amountToRefill = maxStamina - stamina

    edibles
    |> Array.minBy (fun item ->
      closestNeededToReplenish
        item.Food.staminaRecoveredOnConsumption
        amountToRefill
        maxStamina
    )


module CheapestPriceSelector =
  let getCheapestFood (player : Farmer) (edibles : FoodItem array) : FoodItem =
    let f (item : FoodItem) =
      item.Food.sellToStorePrice player.UniqueMultiplayerID

    edibles |> Array.minBy f

module Edibles =
  open CloudyCore.Prelude

  let private isNotForbidden
    (food : Food)
    (forbiddenFood : string array)
    : bool
    =
    not (Array.exists (fun name -> food.Name = name) forbiddenFood)
    // We don't care about food that gives negative health/stamina or nothing at all.
    && food.Edibility > 0


  let makeEdibles
    (playerInventory : Inventories.Inventory)
    (forbiddenFood : string array)
    : FoodItem array
    =
    seq {
      let currentActiveInventoryRowSize = 12

      for i, item in Seq.indexed playerInventory do
        let object = Option.ofObj item |> Option.bind TryCast.toObject
        let isAPriority = i <= currentActiveInventoryRowSize - 1

        match object with
        | Some unprocessedFood ->
          if isNotForbidden unprocessedFood forbiddenFood then
            yield!
              seq {
                {
                  Food = unprocessedFood
                  IsPriority = isAPriority
                }
              }
        | None -> yield! Seq.empty
    }
    |> Seq.toArray

module InventoryFood =
  let tryGetFoodItem (config : ModConfig) (player : Farmer) : FoodItem option =

    let makeEdibles =
      lazy
        Edibles.makeEdibles
          player.Items
          (parsedForbiddenFood config.ForbiddenFoods)

    let getFoodWithHighestPriority (originFood : FoodItem array) =
      let priorities = originFood |> Array.filter (fun f -> f.IsPriority)
      if Array.isEmpty priorities then originFood else priorities

    let getFoodItem pickVitalFoodFromEdibles vitalStat edibles =
      if Array.isEmpty edibles then
        None
      else

        let getFoodWithNextLevelEatingPriority food =
          if config.PriorityStrategySelection = "health_or_stamina" then
            pickVitalFoodFromEdibles vitalStat food
          elif config.PriorityStrategySelection = "cheapest" then
            CheapestPriceSelector.getCheapestFood player food
          else // Off. Just start eating from the active inventory row, left to right.
            Array.head food

        getFoodWithHighestPriority edibles
        |> getFoodWithNextLevelEatingPriority
        |> Some


    match VitalsSelector.getVitalsPriority config player with
    | DoingOK -> None
    | Health vitalStat ->

      getFoodItem
        VitalsSelector.getMostFittingHealthRevitalizingFood
        vitalStat
        (makeEdibles.Force ())


    | Stamina vitalStat ->

      getFoodItem
        VitalsSelector.getMostFittingStaminaRevitalizingFood
        vitalStat
        (makeEdibles.Force ())
