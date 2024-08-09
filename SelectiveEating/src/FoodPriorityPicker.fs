namespace SelectiveEating

open StardewValley
open SelectiveEating.Config

type Food = Object
type FoodItem = { Food : Food ; IsPriority : bool }

type Percentage = | Percentage of int

type VitalStatus = { Current : Percentage ; Max : int }

type VitalsPriority =
  | Health of VitalStatus
  | Stamina of VitalStatus
  | DoingOK

type FoodPriority =
  | Prioritized
  | CheapestPrice
  | Vitals of VitalsPriority

module VitalsSelector =
  let ofPercentage current max =
    int <| (float current / float max) * 100.0

  let private makePercentage current max =
    Percentage <| ofPercentage current max

  let private fromPercentage (Percentage percentage) = percentage

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


  let getMostFittingHealthRevitalizingFood
    (edibles : FoodItem seq)
    (vitalStat : VitalStatus)
    applyPriority
    : FoodItem
    =

    let health = fromPercentage vitalStat.Current
    let maxHealth = vitalStat.Max
    let amountToRefill = maxHealth - health

    edibles
    |> applyPriority
    |> Seq.minBy (fun item ->
      abs (
        ofPercentage (item.Food.healthRecoveredOnConsumption ()) maxHealth
        - amountToRefill
      )
    )

  let getMostFittingStaminaRevitalizingFood
    (edibles : FoodItem seq)
    (vital : VitalStatus)
    applyPriority
    : FoodItem
    =
    let stamina = fromPercentage vital.Current
    let maxStamina = vital.Max
    let amountToRefill = maxStamina - stamina

    edibles
    |> applyPriority
    |> Seq.minBy (fun item ->
      abs (
        ofPercentage (item.Food.staminaRecoveredOnConsumption ()) maxStamina
        - amountToRefill
      )
    )


// TODO: Set which priority is this. Also, should we use this?
module CheapestPriceSelector =
  let getCheapestFood (edibles : FoodItem seq) (player : Farmer) : FoodItem =
    let f (food : FoodItem) =
      food.Food.sellToStorePrice player.UniqueMultiplayerID

    edibles |> Seq.minBy f

module Edibles =
  open CloudyCore.Prelude

  let private isNotForbidden
    (food : Food)
    (forbiddenFood : string array)
    : bool
    =
    not (Seq.exists (fun name -> food.Name = name) forbiddenFood)
    // We don't care about food that gives negative health/stamina or nothing at all.
    && food.Edibility > 0


  let private makeFoodItem
    (food : Food)
    (prioritizedFood : string array)
    : FoodItem
    =
    let isAPriority =
      prioritizedFood |> Seq.exists (fun name -> food.Name = name)

    {
      Food = food
      IsPriority = isAPriority
    }


  let makeEdibles
    (playerInventory : Inventories.Inventory)
    (forbiddenFood : string array)
    (prioritizedFood : string array)
    : FoodItem seq
    =
    seq {
      for item in playerInventory do
        let object = Option.ofObj item |> Option.bind TryCast.toObject

        match object with
        | Some unprocessedFood ->
          if isNotForbidden unprocessedFood forbiddenFood then
            yield! seq { makeFoodItem unprocessedFood prioritizedFood }
        | None -> yield! Seq.empty
    }

module InventoryFood =
  let tryGetFoodItem (config : ModConfig) (player : Farmer) : FoodItem option =
    let withPriority (food : FoodItem seq) =
      if food |> Seq.exists (fun f -> f.IsPriority) then
        food |> Seq.filter (fun f -> f.IsPriority = true)
      else
        food


    // TODO: Bring out of priority picker module and strengthen the list
    // parsing in case of bad user input.
    let forbiddens = config.ForbiddenFoods.Split (',')
    let prioritizeds = config.PrioritizedFoods.Split (',')

    let playerInventory = player.Items

    let makeEdibles =
      lazy Edibles.makeEdibles playerInventory forbiddens prioritizeds

    let getFoodItem pickFromEdibles edibles vitalStat =
      if Seq.isEmpty edibles then
        None
      else
        Some <| pickFromEdibles edibles vitalStat withPriority

    match VitalsSelector.getVitalsPriority config player with
    | DoingOK -> None
    | Health vitalStat ->

      getFoodItem
        VitalsSelector.getMostFittingHealthRevitalizingFood
        (makeEdibles.Force ())
        vitalStat


    | Stamina vitalStat ->

      getFoodItem
        VitalsSelector.getMostFittingStaminaRevitalizingFood
        (makeEdibles.Force ())
        vitalStat
