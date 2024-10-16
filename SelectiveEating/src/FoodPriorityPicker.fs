namespace SelectiveEating

open StardewValley
open CloudyCore.Prelude
open SelectiveEating.API

type FoodObject = Object

module private Edibles =
    open SelectiveEating.API.Edibles

    let makeFoodItem
        (unprocessedFood : FoodObject)
        forbiddenFood
        isPartOfActiveInventoryRow
        =
        seq {
            let food =
                {
                    IsPriority = isPartOfActiveInventoryRow
                    Id = unprocessedFood.ItemId
                    Edibility = unprocessedFood.Edibility
                    HealthRecoveredOnConsumption =
                        unprocessedFood.healthRecoveredOnConsumption ()
                    StaminaRecoveredOnConsumption =
                        unprocessedFood.staminaRecoveredOnConsumption ()
                    SellToStorePrice =
                        unprocessedFood.sellToStorePrice
                            Game1.player.UniqueMultiplayerID
                }

            if isNotForbidden food (parseForbiddenFood forbiddenFood) then
                food
        }

    let makeEdibles
        (playerInventory : Inventories.Inventory)
        (forbiddenFood : string)
        : Food seq
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
    open SelectiveEating.API.VitalsSelector

    let private makeEdibles (config : ModConfig) (player : Farmer) =
        Edibles.makeEdibles player.Items config.ForbiddenFoods

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

    let tryGetOne (config : ModConfig) (player : Farmer) : Food option =
        let items = lazy (makeEdibles config player)

        let getFor replenisher vital =
            let foodItems = items.Force ()

            if Seq.isEmpty foodItems then
                None
            else
                foodItems
                |> Seq.toArray
                |> byHighestPriority
                |> byEatingPriority replenisher config vital
                |> Some


        match getPriority config player with
        | DoingOK -> None
        | Health health -> getFor replenishHealth health
        | Stamina stamina -> getFor replenishStamina stamina
