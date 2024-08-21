namespace SelectiveEating

open StardewValley
open SelectiveEating.Config
open CloudyCore.Prelude.Math

type Food = Object

[<Struct>]
type FoodItem = { Food : Food ; IsPriority : bool }

[<Struct>]
type VitalStatus = { Current : Percentage ; Max : int }

type VitalsPriority =
    | Health of VitalStatus
    | Stamina of VitalStatus
    | DoingOK

module VitalsSelector =

    /// Make percentage of current player's health.
    let makePlayerHealth (player : Farmer) =
        ofPercentage player.health player.maxHealth

    /// Make percentage of current player's stamina.
    let makePlayerStamina (player : Farmer) =
        ofPercentage (int player.stamina) player.MaxStamina

    /// Health will always be the biggest priority for vitals.
    let getVitalsPriority
        (config : ModConfig)
        (player : Farmer)
        : VitalsPriority
        =

        let makeVitalStatus current max =
            {
                Current = makePercentage current max
                Max = max
            }

        if makePlayerHealth player <= config.MinimumHealthToStartAutoEat then
            Health <| makeVitalStatus player.health player.maxHealth
        elif
            makePlayerStamina player <= config.MinimumStaminaToStartAutoEat
        then
            Stamina <| makeVitalStatus (int player.stamina) player.maxHealth
        else
            DoingOK


    let private closestNeededToReplenish
        recoveredAmount
        amountToRefill
        maxStat
        =
        abs (ofPercentage (recoveredAmount ()) maxStat - amountToRefill)

    let getMostHealthReplenishing
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

    let getMostStaminaReplenishing
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


module CheapestSelector =
    let get (player : Farmer) (edibles : FoodItem array) : FoodItem =
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
        not (Array.exists (fun id -> food.ItemId = id) forbiddenFood)

        // We don't care about food that gives negative health/stamina or nothing
        // positive of value at all.
        && food.Edibility > 0


    let makeEdibles
        (playerInventory : Inventories.Inventory)
        (forbiddenFood : string array)
        : FoodItem seq
        =
        seq {
            let currentActiveInventoryRowSize = 12

            for i, item in Seq.indexed playerInventory do
                let isPartOfCurrentActiveInventoryRow =
                    i <= currentActiveInventoryRowSize - 1

                let object = Option.ofObj item |> Option.bind TryCast.toObject

                match object with
                | Some unprocessedFood ->
                    if isNotForbidden unprocessedFood forbiddenFood then
                        yield!
                            seq {
                                {
                                    Food = unprocessedFood
                                    IsPriority =
                                        isPartOfCurrentActiveInventoryRow
                                }
                            }
                | None -> yield! Seq.empty
        }

module InventoryFood =
    let tryGetFoodItem
        (config : ModConfig)
        (player : Farmer)
        : FoodItem option
        =

        let makeEdibles () =
            Edibles.makeEdibles
                player.Items
                (parsedForbiddenFood config.ForbiddenFoods)

        let getOfHighestPriority (originFood : FoodItem array) =
            let priorities = originFood |> Array.filter _.IsPriority
            if Array.isEmpty priorities then originFood else priorities

        let getFoodItem pickMostVitalFromEdibles vitalStat edibles =
            if Array.isEmpty edibles then
                None
            else

                let getOfSelectedEatingPriority food =
                    if
                        config.PriorityStrategySelection = HealthOrStamina
                            .ToString ()
                    then
                        pickMostVitalFromEdibles vitalStat food
                    elif
                        config.PriorityStrategySelection = CheapestFood.ToString ()
                    then
                        CheapestSelector.get player food
                    else // Off. Just start eating from the active inventory row, left to right.
                        Array.head food

                edibles
                |> getOfHighestPriority
                |> getOfSelectedEatingPriority
                |> Some


        match VitalsSelector.getVitalsPriority config player with
        | DoingOK -> None
        | Health vitalStat ->
            getFoodItem
                VitalsSelector.getMostHealthReplenishing
                vitalStat
                (makeEdibles () |> Seq.toArray)

        | Stamina vitalStat ->
            getFoodItem
                VitalsSelector.getMostStaminaReplenishing
                vitalStat
                (makeEdibles () |> Seq.toArray)
