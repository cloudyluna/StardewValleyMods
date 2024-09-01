namespace SelectiveEating

open StardewValley
open CloudyCore.Prelude.Math
open CloudyCore.Prelude

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
    let private makePlayerHealth (player : Farmer) =
        ofPercentage player.health player.maxHealth

    /// Make percentage of current player's stamina.
    let private makePlayerStamina (player : Farmer) =
        ofPercentage (int player.stamina) player.MaxStamina

    let private makeVitalStatus current max =
        {
            Current = makePercentage current max
            Max = max
        }

    let getVitalsPriority (config : ModConfig) (player : Farmer) =
        if makePlayerHealth player <= config.MinimumHealthToStartAutoEat then
            Health ^ makeVitalStatus player.health player.maxHealth
        elif
            makePlayerStamina player <= config.MinimumStaminaToStartAutoEat
        then
            Stamina ^ makeVitalStatus (int player.stamina) player.MaxStamina
        else
            DoingOK


    let private closestNeededToReplenish
        recoveredAmount
        amountToRefill
        maxStat
        =
        abs (ofPercentage (recoveredAmount ()) maxStat - amountToRefill)

    let replenishHealth
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

    let replenishStamina
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


module private CheapestSelector =
    let get (player : Farmer) (edibles : FoodItem array) : FoodItem =
        let f (item : FoodItem) =
            item.Food.sellToStorePrice player.UniqueMultiplayerID

        edibles |> Array.minBy f

module private Edibles =
    let private parsedForbiddenFood (str : string) : string array =
        str.Trim().Split (',', System.StringSplitOptions.RemoveEmptyEntries)
        |> Array.map (fun i -> i.Trim ())

    let private isNotForbidden
        (food : Food)
        (forbiddenFood : string array)
        : bool
        =
        not <| Array.exists (fun id -> food.ItemId = id) forbiddenFood

        // We don't care about food that gives negative health/stamina or nothing
        // positive of value at all.
        && food.Edibility > 0

    let private makeFoodItem
        unprocessedFood
        forbiddenFood
        isPartOfActiveInventoryRow
        =
        seq {
            if
                isNotForbidden
                    unprocessedFood
                    (parsedForbiddenFood forbiddenFood)
            then
                {
                    Food = unprocessedFood
                    IsPriority = isPartOfActiveInventoryRow
                }
        }


    let makeEdibles
        (playerInventory : Inventories.Inventory)
        (forbiddenFood : string)
        : FoodItem seq
        =
        seq {
            let activeInventoryRowSize = 12

            for i, item in Seq.indexed playerInventory do
                let isPartOfCurrentActiveInventoryRow =
                    i <= activeInventoryRowSize - 1

                let object = Option.ofObj item |> Option.bind TryCast.toObject

                match object with
                | Some unprocessedFood ->
                    yield!
                        makeFoodItem
                            unprocessedFood
                            forbiddenFood
                            isPartOfCurrentActiveInventoryRow
                | None -> yield! Seq.empty
        }

module InventoryFood =
    open VitalsSelector

    let private makeEdibles (config : ModConfig) (player : Farmer) =
        Edibles.makeEdibles player.Items config.ForbiddenFoods

    let private byHighestPriority (originFood : FoodItem array) =
        let priorities = originFood |> Array.filter _.IsPriority
        if Array.isEmpty priorities then originFood else priorities

    let private byEatingPriority
        getVital
        (config : ModConfig)
        player
        vitalStat
        food
        =
        if config.PriorityStrategySelection = HealthOrStamina.ToString () then
            getVital vitalStat food
        elif config.PriorityStrategySelection = CheapestFood.ToString () then
            CheapestSelector.get player food
        else // Off. Just start eating from the active inventory row, left to right.
            Array.head food

    let tryGetOne (config : ModConfig) (player : Farmer) : FoodItem option =
        let items = lazy (makeEdibles config player)

        let getFor replenisher vital =
            let foodItems = items.Force ()

            if Seq.isEmpty foodItems then
                None
            else
                foodItems
                |> Seq.toArray
                |> byHighestPriority
                |> byEatingPriority replenisher config player vital
                |> Some


        match getVitalsPriority config player with
        | DoingOK -> None
        | Health health -> getFor replenishHealth health
        | Stamina stamina -> getFor replenishStamina stamina
