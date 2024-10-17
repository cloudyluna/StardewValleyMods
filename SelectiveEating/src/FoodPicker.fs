namespace SelectiveEating.View

open StardewValley
open CloudyCore.Prelude
open SelectiveEating.API

type FoodObject = Object

module private Edibles =
    open SelectiveEating.API.Edibles


    let private makeFoodItem
        (food : Food)
        (foodObj : FoodObject)
        forbiddenFood
        =
        seq {
            if isNotForbidden food (parseForbiddenFood forbiddenFood) then
                yield food, foodObj
        }

    let makeEdibles
        (forbiddenFood : string)
        (playerInventory : Inventories.Inventory)
        (uniqueMultiplayerId : int64)
        : (Food * FoodObject) seq
        =
        seq {
            let activeInventoryRowSize = 12

            for i, item in Seq.indexed playerInventory do
                let isPartOfCurrentActiveInventoryRow =
                    i <= activeInventoryRowSize - 1

                let object = Option.ofObj item |> Option.bind TryCast.toObject

                match object with
                | Some foodObj ->
                    let food =
                        {
                            IsPriority = isPartOfCurrentActiveInventoryRow
                            Id = foodObj.ItemId
                            Edibility = foodObj.Edibility
                            HealthRecoveredOnConsumption =
                                foodObj.healthRecoveredOnConsumption ()
                            StaminaRecoveredOnConsumption =
                                foodObj.staminaRecoveredOnConsumption ()
                            SellToStorePrice =
                                foodObj.sellToStorePrice uniqueMultiplayerId
                        }

                    yield! makeFoodItem food foodObj forbiddenFood

                | None -> yield! Seq.empty
        }

module InventoryFood =
    open SelectiveEating.API.VitalsSelector
    open SelectiveEating.Config

    let private makeEdibles (config : ModConfig) (player : Farmer) =
        Edibles.makeEdibles
            config.ForbiddenFoods
            player.Items
            player.UniqueMultiplayerID

    let private byHighestPriority (originFood : Food array) =
        let priorities = originFood |> Array.filter (fun fi -> fi.IsPriority)

        if Array.isEmpty priorities then originFood else priorities

    let private byEatingPriority
        getVital
        (config : ModConfig)
        (vitalStat : VitalStatus)
        (food : Food array)
        : Food
        =
        if config.PriorityStrategySelection = HealthOrStamina.ToString () then
            getVital vitalStat food
        elif config.PriorityStrategySelection = CheapestFood.ToString () then
            CheapestSelector.get food
        else // Off. Just start eating from the active inventory row, left to right.
            Array.head food

    let private getPriority config (player : Farmer) =
        getVitalsPriority
            config
            {
                Current = player.health
                Max = player.maxHealth
            }
            {
                Current = int player.stamina
                Max = player.MaxStamina
            }

    let private getFor
        config
        (items : Lazy<seq<Food * FoodObject>>)
        replenisher
        vital
        =
        let foodItems = items.Force ()

        if Seq.isEmpty foodItems then
            None
        else
            let foods = foodItems |> Seq.rev |> Map.ofSeq

            let chosenFood =
                foods.Keys
                |> Seq.toArray
                |> byHighestPriority
                |> byEatingPriority replenisher config vital

            Some <| Map.find chosenFood foods


    let tryGetOne (config : ModConfig) (player : Farmer) : FoodObject option =
        let items = lazy (makeEdibles config player)
        let getOne = getFor config items

        match getPriority config player with
        | DoingOK -> None
        | Health health -> getOne replenishHealth health
        | Stamina stamina -> getOne replenishStamina stamina
